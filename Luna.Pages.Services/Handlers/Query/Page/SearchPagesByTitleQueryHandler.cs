using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Repositories.Repositories.Page.Query;
using Luna.Pages.Services.Queries.Page;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Query.Page;

public class SearchPagesByTitleQueryHandler: PageQueryHandlerBase, IRequestHandler<SearchPagesByTitleQuery, IEnumerable<PageDomain>>
{
	public SearchPagesByTitleQueryHandler(IPageQueryRepository pageQueryRepository) : base(pageQueryRepository)
	{
	}

	public async Task<IEnumerable<PageDomain>> Handle(SearchPagesByTitleQuery request, CancellationToken cancellationToken)
	{
		IEnumerable<PageDatabase> pages = await PageQueryRepository.SearchPagesByTitleAsync(request.SearchTerm, request.WorkspaceId, 50, cancellationToken);

		return pages.Select(PageDomain.FromDatabase);
	}
}