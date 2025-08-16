using Luna.Pages.Models.Blank.Models;

namespace Luna.Pages.API.Hubs;

public interface IPageHub
{
	Task JoinPage(Guid pageId);
	Task LeavePage(Guid pageId);
	Task Pong(DateTime timestamp);
	Task GetPageData(Guid pageId);
	Task UpdatePage(Guid pageId, PatchPageBlank patchPageBlank);
	Task UpdatePageContent(Guid pageId, UpdatePageContentBlank pageContentBlank);
	Task GetPageComments(Guid pageId);
	Task CreateComment(Guid pageId, CreatePageCommentBlank createPageCommentBlank);
	Task UpdateComment(Guid commentId, CreatePageCommentBlank createPageCommentBlank);
	Task DeleteComment(Guid commentId);
}