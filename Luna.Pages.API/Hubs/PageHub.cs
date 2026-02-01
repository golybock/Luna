using Luna.Pages.Models.Blank.Models;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Models.View.Additional;
using Luna.Pages.Models.View.Models;
using Luna.Pages.Repositories.Repositories.Session;
using Luna.Pages.Services.Services.PageService;
using Luna.Tools.SharedModels.Models.API;
using Luna.Tools.Web;
using Luna.Users.gRPC.Client.Services;
using Luna.Users.Models.Domain.Models;
using Luna.Users.Models.View.Models;
using Microsoft.AspNetCore.SignalR;

namespace Luna.Pages.API.Hubs;

public class PageHub : HubBase, IPageHub
{
	private readonly IPageService _pageService;
	private readonly IUserServiceClient _userServiceClient;
	private readonly ILogger<PageHub> _logger;
	private readonly ISessionCacheRepository _sessionCacheRepository;

	public PageHub(ILogger<PageHub> logger, IPageService pageService, IUserServiceClient userServiceClient,
		ISessionCacheRepository sessionCacheRepository)
	{
		_logger = logger;
		_pageService = pageService;
		_userServiceClient = userServiceClient;
		_sessionCacheRepository = sessionCacheRepository;
	}

	public override async Task OnConnectedAsync()
	{
		if (UserId == null)
		{
			Context.Abort();
			return;
		}

		_logger.LogInformation("User {UserId} connected with connection {ContextConnectionId}", UserId,
			Context.ConnectionId);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		string connectionId = Context.ConnectionId;

		if (UserId.HasValue)
		{
			string? pageId = await _sessionCacheRepository.GetConnectionPageAsync(connectionId);

			if (pageId != null)
			{
				await LeavePageInternal(pageId, UserId.Value.ToString(), connectionId);
			}
		}

		_logger.LogInformation("User {UserId} disconnected", UserId);
		await base.OnDisconnectedAsync(exception);
	}

	private async Task LeavePageInternal(string pageId, string userId, string connectionId)
	{
		try
		{
			string group = $"page_{pageId}";
			await Groups.RemoveFromGroupAsync(connectionId, group);

			await _sessionCacheRepository.RemoveUserFromPageAsync(pageId, userId);
			await _sessionCacheRepository.RemoveUserCursorAsync(pageId, userId);
			await _sessionCacheRepository.RemoveConnectionPageAsync(connectionId);

			List<UserCursorDomain> cursors = (await _sessionCacheRepository.GetPageCursorsAsync(pageId)).ToList();
			List<UserDomain> users = (await _sessionCacheRepository.GetPageUsersAsync(pageId)).ToList();

			List<UserCursorView> cursorViews = cursors
				.Select(c => c.ToView()).ToList();

			List<UserView> userViews = users
				.Select(c => c.ToView()).ToList();

			await Clients.OthersInGroup(group).SendAsync("CursorSet", cursorViews);
			await Clients.OthersInGroup(group).SendAsync("UsersSet", userViews);

			// await Clients.Group(group).SendAsync("UserLeftPage", new {userId, pageId});

			_logger.LogInformation("User {UserId} left page {PageId}", userId, pageId);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error leaving page {PageId} for user {UserId}", pageId, userId);
		}
	}

	public async Task JoinPage(string pageId)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		string group = $"page_{pageId}";
		string connectionId = Context.ConnectionId;

		string? oldPageId = await _sessionCacheRepository.GetConnectionPageAsync(connectionId);
		if (oldPageId != null && oldPageId != pageId)
		{
			await LeavePageInternal(oldPageId, UserId.Value.ToString(), connectionId);
		}

		await Groups.AddToGroupAsync(connectionId, group);

		UserDomain? userDomain = null;

		try
		{
			userDomain = await _userServiceClient.GetUserByIdAsync(UserId.Value);
		}
		catch (Exception e)
		{
			_logger.LogError("Error get userDomain, userId: ${UserId}", UserId.Value.ToString());
		}

		await _sessionCacheRepository.AddUserToPageAsync(pageId, UserId.Value.ToString(), userDomain);
		await _sessionCacheRepository.SetConnectionPageAsync(connectionId, pageId);

		List<UserDomain> users = (await _sessionCacheRepository.GetPageUsersAsync(pageId)).ToList();

		List<UserView> userViews = users
			.Select(c => c.ToView()).ToList();

		await Clients.Group(group).SendAsync("UsersSet", userViews);
		_logger.LogInformation("User {UserId} joined page {PageId}", UserId, pageId);
	}

	public async Task LeavePage(string pageId)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		await LeavePageInternal(pageId, UserId.Value.ToString(), Context.ConnectionId);
	}

	public async Task Pong(DateTime timestamp)
	{
		await Clients.Caller.SendAsync("Pong", new {sent = timestamp, server = DateTime.UtcNow});
	}

	public async Task GetPageData(string pageId)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		GetRequest getRequest = new GetRequest {Id = new Guid(pageId), UserId = UserId.Value};
		PageFullView? page = await _pageService.GetPageFullViewAsync(getRequest);
		await Clients.Caller.SendAsync("PageData", page);
	}

	public async Task SetCursor(string pageId, UserCursorBlank userCursorBlank)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		string group = $"page_{pageId}";

		List<UserCursorDomain> cursors = (await _sessionCacheRepository.GetPageCursorsAsync(pageId)).ToList();

		UserCursorDomain? existingCursor = cursors.FirstOrDefault(c => c.UserId == UserId.Value.ToString());

		if (existingCursor != null)
		{
			UserCursorDomain cursorDomain = UserCursorDomain.FromBlank(userCursorBlank, UserId.Value.ToString(),
				existingCursor.UserDisplayName);

			cursors.Remove(existingCursor);
			cursors.Add(cursorDomain);

			await _sessionCacheRepository.UpsertUserCursorAsync(pageId, cursorDomain);
		}
		else
		{
			try
			{
				UserDomain? user = await _sessionCacheRepository.GetPageUserByIdAsync(pageId, UserId.Value.ToString());

				UserCursorDomain cursorDomain =
					UserCursorDomain.FromBlank(userCursorBlank, UserId.Value.ToString(), user?.DisplayName ?? "Unknow");

				cursors.Add(cursorDomain);

				await _sessionCacheRepository.UpsertUserCursorAsync(pageId, cursorDomain);
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Error getting user details for user {UserId}", UserId.Value);
			}
		}

		List<UserCursorView> allCursors = cursors
			.Select(c => c.ToView()).ToList();

		await Clients.OthersInGroup(group).SendAsync("CursorSet", allCursors);
	}

	public async Task UpdatePage(string pageId, PatchPageBlank patchPageBlank)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		UpdateRequest<PatchPageBlank> request = new UpdateRequest<PatchPageBlank>
		{
			ObjectId = new Guid(pageId),
			UserId = UserId.Value,
			Blank = patchPageBlank
		};

		bool result = await _pageService.UpdatePageAsync(request);

		string group = $"page_{pageId}";
		await Clients.OthersInGroup(group).SendAsync("PageUpdated", new {pageId});
	}

	public async Task UpdatePageContent(string pageId, UpdatePageContentBlank pageContentBlank)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		UpdateRequest<UpdatePageContentBlank> request = new UpdateRequest<UpdatePageContentBlank>
		{
			ObjectId = new Guid(pageId),
			UserId = UserId.Value,
			Blank = pageContentBlank
		};

		await _pageService.UpdatePageContentAsync(request);

		GetRequest getRequest = new GetRequest {Id = new Guid(pageId), UserId = UserId.Value};
		PageFullView? pageFull = await _pageService.GetPageFullViewAsync(getRequest);

		object? document = pageFull?.PageVersionView?.Document;
		int version = pageFull?.PageVersionView?.Version ?? 0;
		DateTime? updatedAt = pageFull?.PageVersionView?.UpdatedAt;

		string group = $"page_{pageId}";
		await Clients.OthersInGroup(group)
			.SendAsync("PageContentUpdated", new {pageId, document, version, updatedAt});
	}

	public async Task GetPageComments(string pageId)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		GetRequest getRequest = new GetRequest {Id = new Guid(pageId), UserId = UserId.Value};
		IEnumerable<PageCommentView> comments = await _pageService.GetPageCommentsAsync(getRequest);
		await Clients.Caller.SendAsync("PageComments", new {pageId, comments});
	}

	public async Task CreateComment(string pageId, CreatePageCommentBlank createPageCommentBlank)
	{
		string group = $"page_{pageId}";
		
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		createPageCommentBlank.PageId = new Guid(pageId);
		BlankRequest<CreatePageCommentBlank> request = new BlankRequest<CreatePageCommentBlank>
		{
			UserId = UserId.Value,
			Blank = createPageCommentBlank
		};
		await _pageService.CreatePageCommentAsync(request);

		GetRequest getRequest = new GetRequest {Id = new Guid(pageId), UserId = UserId.Value};
		IEnumerable<PageCommentView> comments = await _pageService.GetPageCommentsAsync(getRequest);
		await Clients.Group(group).SendAsync("PageCommentsUpdated", new {pageId, comments});
	}

	public async Task UpdateComment(string commentId, CreatePageCommentBlank createPageCommentBlank)
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
			ObjectId = new Guid(commentId),
			UserId = UserId.Value,
			Blank = patch
		};

		await _pageService.UpdatePageCommentAsync(request);
		await Clients.Caller.SendAsync("CommentUpdated", new {commentId});
	}

	public async Task DeleteComment(string commentId)
	{
		if (!UserId.HasValue)
		{
			Context.Abort();
			return;
		}

		DeleteRequest request = new DeleteRequest {ObjectId = new Guid(commentId), UserId = UserId.Value};
		await _pageService.DeletePageCommentAsync(request);
		await Clients.Caller.SendAsync("CommentDeleted", new {commentId});
	}
}