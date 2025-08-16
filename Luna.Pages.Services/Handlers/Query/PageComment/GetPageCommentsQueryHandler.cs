using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Models.View.Models;
using Luna.Pages.Repositories.Repositories.PageComment;
using Luna.Pages.Services.Queries.PageComment;
using MediatR;

namespace Luna.Pages.Services.Handlers.Query.PageComment;

public class GetPageCommentsQueryHandler : PageCommentQueryHandlerBase, IRequestHandler<GetPageCommentsQuery, IEnumerable<PageCommentDomain>>
{
	public GetPageCommentsQueryHandler(IPageCommentQueryRepository pageCommentQueryRepository)
		: base(pageCommentQueryRepository) { }

	public async Task<IEnumerable<PageCommentDomain>> Handle(GetPageCommentsQuery request, CancellationToken cancellationToken)
	{
		IEnumerable<PageCommentDatabase> commentsDatabase =  await PageCommentQueryRepository.GetPageComments(request.PageId, cancellationToken);

		return commentsDatabase.Select(PageCommentDomain.FromDatabase);
	}
}