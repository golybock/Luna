using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Models.View.Additional;
using Luna.Pages.Repositories.Repositories.Page.Query;
using Luna.Pages.Services.Queries.Page;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Query.Page;

public class GetArchivedPagesQueryHandler : PageQueryHandlerBase, IRequestHandler<GetArchivedPagesQuery, IEnumerable<PageDomain>>
{
	public GetArchivedPagesQueryHandler(IPageQueryRepository pageQueryRepository) : base(pageQueryRepository) { }

	public async Task<IEnumerable<PageDomain>> Handle(GetArchivedPagesQuery request, CancellationToken cancellationToken)
	{
		IEnumerable<PageDatabase> pages = await PageQueryRepository.GetArchivedPagesAsync(request.WorkspaceId, cancellationToken);

		IEnumerable<PageDomain> pagesDomainWithChildren = BuildHierarchy(pages);

		return pagesDomainWithChildren;
	}
}