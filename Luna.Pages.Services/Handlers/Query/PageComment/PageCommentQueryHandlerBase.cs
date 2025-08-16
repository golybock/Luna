using Luna.Pages.Repositories.Repositories.PageComment;

namespace Luna.Pages.Services.Handlers.Query.PageComment;

public abstract class PageCommentQueryHandlerBase
{
	protected readonly IPageCommentQueryRepository PageCommentQueryRepository;

	protected PageCommentQueryHandlerBase(IPageCommentQueryRepository pageCommentQueryRepository)
	{
		PageCommentQueryRepository = pageCommentQueryRepository;
	}
}