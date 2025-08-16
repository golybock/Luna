using Luna.Pages.Models.Database.Models;

namespace Luna.Pages.Repositories.Repositories.PageComment;

public interface IPageCommentCommandRepository
{
	Task<bool> CreatePageComment(PageCommentDatabase pageCommentDatabase);
	Task<bool> PatchPageComment(Guid id, Dictionary<string, object?> updates);
	Task<bool> DeletePageComment(Guid id);
}