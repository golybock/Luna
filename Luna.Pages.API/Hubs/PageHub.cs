using System.Collections.Concurrent;
using Luna.Pages.Models.Blank.Models;
using Luna.Pages.Models.View.Models;
using Luna.Pages.Services.Services.PageService;
using Luna.Tools.SharedModels.Models.API;
using Luna.Tools.Web;
using Microsoft.AspNetCore.SignalR;

namespace Luna.Pages.API.Hubs;

public class PageHub : HubBase, IPageHub
{
	private readonly IPageService _pageService;
	private readonly ILogger<PageHub> _logger;

	// Словарь для отслеживания пользователей на конкретных страницах
	private static readonly ConcurrentDictionary<string, HashSet<Guid>> PageUsers = new();
	// Словарь для отслеживания активных страниц пользователей (connectionId -> pageId)
	private static readonly ConcurrentDictionary<string, string> UserPages = new();

	public PageHub(ILogger<PageHub> logger, IPageService pageService)
	{
		_logger = logger;
		_pageService = pageService;
	}

	public override async Task OnConnectedAsync()
	{
		if (UserId == null)
		{
			Context.Abort();
			return;
		}

		_logger.LogInformation("User {UserId} connected with connection {ContextConnectionId}", UserId, Context.ConnectionId);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception exception)
	{
		string connectionId = Context.ConnectionId;

		if (UserId.HasValue && UserPages.TryGetValue(connectionId, out string? pageId))
		{
			await LeavePageInternal(pageId, UserId.Value, connectionId);
		}

		_logger.LogInformation("User {UserId} disconnected", UserId);
		await base.OnDisconnectedAsync(exception);
	}

	private async Task LeavePageInternal(string pageId, Guid userId, string connectionId)
	{
		try
		{
			string group = $"page_{pageId}";
			await Groups.RemoveFromGroupAsync(connectionId, group);

			if (PageUsers.TryGetValue(pageId, out var users))
			{
				users.Remove(userId);
				if (users.Count == 0)
				{
					PageUsers.TryRemove(pageId, out _);
				}
			}

			UserPages.TryRemove(connectionId, out _);

			await Clients.Group(group).SendAsync("UserLeftPage", new { userId, pageId });

			_logger.LogInformation("User {UserId} left page {PageId}", userId, pageId);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error leaving page {PageId} for user {UserId}", pageId, userId);
		}
	}

	public async Task JoinPage(Guid pageId)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		string pid = pageId.ToString();
		string group = $"page_{pid}";
		string connectionId = Context.ConnectionId;

		// Если уже на другой странице — выйдем из неё
		if (UserPages.TryGetValue(connectionId, out string? oldPageId) && oldPageId != pid)
		{
			await LeavePageInternal(oldPageId, UserId.Value, connectionId);
		}

		await Groups.AddToGroupAsync(connectionId, group);

		PageUsers.AddOrUpdate(pid, _ => [UserId.Value],
			(_, set) => { set.Add(UserId.Value); return set; });

		UserPages[connectionId] = pid;

		await Clients.Group(group).SendAsync("UserJoinedPage", new { userId = UserId.Value, pageId = pid });
		_logger.LogInformation("User {UserId} joined page {PageId}", UserId, pid);
	}

	public async Task LeavePage(Guid pageId)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}
		await LeavePageInternal(pageId.ToString(), UserId.Value, Context.ConnectionId);
	}

	public async Task Pong(DateTime timestamp)
	{
		await Clients.Caller.SendAsync("Pong", new { sent = timestamp, server = DateTime.UtcNow });
	}

	public async Task GetPageData(Guid pageId)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		GetRequest getRequest = new GetRequest { Id = pageId, UserId = UserId.Value };
		PageFullView? page = await _pageService.GetPageFullViewAsync(getRequest);
		await Clients.Caller.SendAsync("PageData", page);
	}

	public async Task UpdatePage(Guid pageId, PatchPageBlank patchPageBlank)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		UpdateRequest<PatchPageBlank> request = new UpdateRequest<PatchPageBlank>
		{
			ObjectId = pageId,
			UserId = UserId.Value,
			Blank = patchPageBlank
		};

		bool rs = await _pageService.UpdatePageAsync(request);

		string group = $"page_{pageId}";
		await Clients.OthersInGroup(group).SendAsync("PageUpdated", new { pageId });
	}

	public async Task UpdatePageContent(Guid pageId, UpdatePageContentBlank pageContentBlank)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		UpdateRequest<UpdatePageContentBlank> request = new UpdateRequest<UpdatePageContentBlank>
		{
			ObjectId = pageId,
			UserId = UserId.Value,
			Blank = pageContentBlank
		};

		await _pageService.UpdatePageContentAsync(request);

		// После обновления вернем свежие блоки
		GetRequest getRequest = new GetRequest { Id = pageId, UserId = UserId.Value };
		IEnumerable<PageBlockView> blocks = await _pageService.GetPageBlocksAsync(getRequest);
		await Clients.OthersInGroup($"page_{pageId}").SendAsync("PageContentUpdated", new { pageId, blocks });
	}

	public async Task GetPageComments(Guid pageId)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}
		GetRequest getRequest = new GetRequest { Id = pageId, UserId = UserId.Value };
		var comments = await _pageService.GetPageCommentsAsync(getRequest);
		await Clients.Caller.SendAsync("PageComments", new { pageId, comments });
	}

	public async Task CreateComment(Guid pageId, CreatePageCommentBlank createPageCommentBlank)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		createPageCommentBlank.PageId = pageId;
		BlankRequest<CreatePageCommentBlank> request = new BlankRequest<CreatePageCommentBlank>
		{
			UserId = UserId.Value,
			Blank = createPageCommentBlank
		};
		await _pageService.CreatePageCommentAsync(request);

		GetRequest getRequest = new GetRequest { Id = pageId, UserId = UserId.Value };
		IEnumerable<PageCommentView> comments = await _pageService.GetPageCommentsAsync(getRequest);
		await Clients.Group($"page_{pageId}").SendAsync("PageCommentsUpdated", new { pageId, comments });
	}

	public async Task UpdateComment(Guid commentId, CreatePageCommentBlank createPageCommentBlank)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		PatchPageCommentBlank patch = new PatchPageCommentBlank
		{
			Content = createPageCommentBlank.Content,
			ParentId = createPageCommentBlank.ParentId,
			BlockId = createPageCommentBlank.BlockId,
			Reactions = createPageCommentBlank.Reactions
		};

		UpdateRequest<PatchPageCommentBlank> request = new UpdateRequest<PatchPageCommentBlank>
		{
			ObjectId = commentId,
			UserId = UserId.Value,
			Blank = patch
		};

		await _pageService.UpdatePageCommentAsync(request);
		await Clients.Caller.SendAsync("CommentUpdated", new { commentId });
	}

	public async Task DeleteComment(Guid commentId)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		DeleteRequest request = new DeleteRequest { ObjectId = commentId, UserId = UserId.Value };
		await _pageService.DeletePageCommentAsync(request);
		await Clients.Caller.SendAsync("CommentDeleted", new { commentId });
	}
}