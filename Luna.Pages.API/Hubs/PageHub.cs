using System.Collections.Concurrent;
using Luna.Pages.Models.Blank.Models;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Models.View.Additional;
using Luna.Pages.Models.View.Models;
using Luna.Pages.Services.Services.PageService;
using Luna.Tools.SharedModels.Models.API;
using Luna.Tools.Web;
using Luna.Users.gRPC.Client.Services;
using Luna.Users.Models.Domain.Models;
using Microsoft.AspNetCore.SignalR;

namespace Luna.Pages.API.Hubs;

public class PageHub : HubBase, IPageHub
{
	private readonly IPageService _pageService;
	private readonly IUserServiceClient _userServiceClient;
	private readonly ILogger<PageHub> _logger;

	// page_[pageId] - HashSet[userIds] - пользователи на конкретных страницах
	private static readonly ConcurrentDictionary<string, HashSet<Guid>> PageUsersId = new();
	// page_[pageId] - IEnumerable[userCursor] - курсоры пользователей на конкретных страницах
	private static readonly ConcurrentDictionary<string, List<UserCursorDomain>> UserPageCursors = new();

	// Словарь для отслеживания активных страниц пользователей (connectionId -> pageId)
	private static readonly ConcurrentDictionary<string, string> UserPages = new();

	public PageHub(ILogger<PageHub> logger, IPageService pageService, IUserServiceClient userServiceClient)
	{
		_logger = logger;
		_pageService = pageService;
		_userServiceClient = userServiceClient;
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

	public override async Task OnDisconnectedAsync(Exception? exception)
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

			if (PageUsersId.TryGetValue(pageId, out HashSet<Guid>? users))
			{
				users.Remove(userId);
				if (users.Count == 0)
				{
					PageUsersId.TryRemove(pageId, out _);
				}
			}

			if (UserPageCursors.TryGetValue(group, out List<UserCursorDomain>? userCursors))
			{
				UserCursorDomain? user = userCursors.FirstOrDefault(user => user.UserId == userId);
				if (user != null) userCursors.Remove(user);
				if (userCursors.Count == 0)
				{
					PageUsersId.TryRemove(pageId, out _);
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

		PageUsersId.AddOrUpdate(pid, _ => [UserId.Value],
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

	public async Task SetCursor(Guid pageId, UserCursorBlank userCursorBlank)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		string pid = pageId.ToString();
		string group = $"page_{pid}";

		List<UserCursorDomain> cursors = UserPageCursors.GetOrAdd(pid, _ => new List<UserCursorDomain>());

		UserCursorDomain? existingCursor = cursors.FirstOrDefault(c => c.User?.Id == UserId.Value);

		if (existingCursor != null)
		{
			UserCursorDomain cursorDomain = UserCursorDomain.FromBlank(userCursorBlank, existingCursor.User, UserId.Value);

			// удаляем старый курсор, добавляем новый
			cursors.Remove(existingCursor);
			cursors.Add(cursorDomain);
		}
		else
		{
			try
			{
				UserDomain? userDomain = await _userServiceClient.GetUserByIdAsync(UserId.Value);

				UserCursorDomain cursorDomain = UserCursorDomain.FromBlank(userCursorBlank, userDomain, UserId.Value);

				// добавляем новый курсор с данными пользователя
				UserPageCursors[pid].Add(cursorDomain);
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Error getting user details for user {UserId}", UserId.Value);
			}
		}

		List<UserCursorView> allCursors = cursors
			.Select(c => c.ToView()).
			ToList();

		await Clients.OthersInGroup(group).SendAsync("CursorSet", allCursors);
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