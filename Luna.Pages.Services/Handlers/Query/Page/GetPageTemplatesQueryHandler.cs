using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Repositories.Repositories.Page.Query;
using Luna.Pages.Services.Queries.Page;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Query.Page;

public class GetPageTemplatesQueryHandler : PageQueryHandlerBase, IRequestHandler<GetPageTemplatesQuery, IEnumerable<PageDomain>>
{
	public GetPageTemplatesQueryHandler(IPageQueryRepository pageQueryRepository) : base(pageQueryRepository)
	{
	}

	public async Task<IEnumerable<PageDomain>> Handle(GetPageTemplatesQuery request, CancellationToken cancellationToken)
	{
		IEnumerable<PageDatabase> pages = await PageQueryRepository.GetPageTemplatesAsync(request.WorkspaceId, cancellationToken);

		return pages.Select(PageDomain.FromDatabase);
	}
}