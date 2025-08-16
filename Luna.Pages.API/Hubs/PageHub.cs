using System.Collections.Concurrent;
using Luna.Pages.Models.Blank.Models;
using Luna.Pages.Services.Services;
using Luna.Tools.Web;
using Microsoft.AspNetCore.SignalR;

namespace Luna.Pages.API.Hubs;

public class PageHub : HubBase, IPageHub
{
	private readonly IPageService _pageService;
	private readonly ILogger<PageHub> _logger;

	// Словарь для отслеживания пользователей на конкретных страницах
	private static readonly ConcurrentDictionary<string, HashSet<Guid>> PageUsers = new();
	// Словарь для отслеживания активных страниц пользователей
	private static readonly ConcurrentDictionary<string, string> UserPages = new();

	public PageHub(ILogger<PageHub> logger, IPageService pageService)
	{
		_logger = logger;
		_pageService = pageService;
	}

	public override async Task OnConnectedAsync()
	{
		Guid? userId = GetUserIdFromCookie();

		if (userId == null)
		{
			Context.Abort();
			return;
		}

		_logger.LogInformation("User {UserId} connected with connection {ContextConnectionId}", userId, Context.ConnectionId);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception exception)
	{
		Guid userId = GetUserIdFromCookie()!.Value;
		string connectionId = Context.ConnectionId;

		// Удаляем пользователя из текущей страницы
		if (UserPages.TryGetValue(connectionId, out string? pageId))
		{
			await LeavePageInternal(pageId, userId, connectionId);
		}

		_logger.LogInformation("User {UserId} disconnected", userId);
		await base.OnDisconnectedAsync(exception);
	}

	private async Task LeavePageInternal(string pageId, Guid userId, string connectionId)
	{
		try
		{
			// Удаляем из группы
			await Groups.RemoveFromGroupAsync(connectionId, $"page_{pageId}");

			// Удаляем пользователя из списка активных на странице
			if (PageUsers.TryGetValue(pageId, out var users))
			{
				users.Remove(userId);
				if (users.Count == 0)
				{
					PageUsers.TryRemove(pageId, out _);
				}
			}

			UserPages.TryRemove(connectionId, out _);

			await Clients.Group($"page_{pageId}").SendAsync("UserLeftPage", userId, pageId);

			_logger.LogInformation("User {UserId} left page {PageId}", userId, pageId);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error leaving page {PageId} for user {UserId}", pageId, userId);
		}
	}

	public async Task JoinPage(Guid pageId)
	{
		throw new NotImplementedException();
	}

	public async Task LeavePage(Guid pageId)
	{
		throw new NotImplementedException();
	}

	public async Task Pong(DateTime timestamp)
	{
		throw new NotImplementedException();
	}

	public async Task GetPageData(Guid pageId)
	{
		throw new NotImplementedException();
	}

	public async Task UpdatePage(Guid pageId, PatchPageBlank patchPageBlank)
	{
		throw new NotImplementedException();
	}

	public async Task UpdatePageContent(Guid pageId, UpdatePageContentBlank pageContentBlank)
	{
		throw new NotImplementedException();
	}

	public async Task GetPageComments(Guid pageId)
	{
		throw new NotImplementedException();
	}

	public async Task CreateComment(Guid pageId, CreatePageCommentBlank createPageCommentBlank)
	{
		throw new NotImplementedException();
	}

	public async Task UpdateComment(Guid commentId, CreatePageCommentBlank createPageCommentBlank)
	{
		throw new NotImplementedException();
	}

	public async Task DeleteComment(Guid commentId)
	{
		throw new NotImplementedException();
	}
}