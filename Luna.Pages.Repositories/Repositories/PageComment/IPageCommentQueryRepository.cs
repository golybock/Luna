using Luna.Pages.Models.Database.Models;

namespace Luna.Pages.Repositories.Repositories.PageComment;

public interface IPageCommentQueryRepository
{
	Task<PageCommentDatabase?> GetPageComment(Guid commentId, CancellationToken cancellationToken);

	Task<IEnumerable<PageCommentDatabase>> GetPageCommentsByIds(IEnumerable<Guid> commentIds, CancellationToken cancellationToken);

	Task<IEnumerable<PageCommentDatabase>> GetPageComments(Guid pageId, CancellationToken cancellationToken);
}