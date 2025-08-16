using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Repositories.Repositories.Page.Query;
using Luna.Pages.Services.Queries.Page;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Query.Page;

public class GetWorkspacePagesQueryHandler : PageQueryHandlerBase, IRequestHandler<GetWorkspacePagesQuery, IEnumerable<PageDomain>>
{
	public GetWorkspacePagesQueryHandler(IPageQueryRepository pageQueryRepository) : base(pageQueryRepository)
	{
	}

	public async Task<IEnumerable<PageDomain>> Handle(GetWorkspacePagesQuery request, CancellationToken cancellationToken)
	{
		IEnumerable<PageDatabase> pages = await PageQueryRepository.GetWorkspacePagesAsync(request.WorkspaceId, false, cancellationToken);

		return pages.Select(PageDomain.FromDatabase);
	}
}