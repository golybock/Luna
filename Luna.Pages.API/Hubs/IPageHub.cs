using Luna.Pages.Models.Blank.Models;

namespace Luna.Pages.API.Hubs;

public interface IPageHub
{
	Task JoinPage(string pageId);
	Task LeavePage(string pageId);
	Task Pong(DateTime timestamp);
	Task GetPageData(string pageId);
	Task SetCursor(string pageId, UserCursorBlank userCursorBlank);
	Task UpdatePage(string pageId, PatchPageBlank patchPageBlank);
	Task UpdatePageContent(string pageId, UpdatePageContentBlank pageContentBlank);
	Task GetPageComments(string pageId);
	Task CreateComment(string pageId, CreatePageCommentBlank createPageCommentBlank);
	Task UpdateComment(string commentId, CreatePageCommentBlank createPageCommentBlank);
	Task DeleteComment(string commentId);
}