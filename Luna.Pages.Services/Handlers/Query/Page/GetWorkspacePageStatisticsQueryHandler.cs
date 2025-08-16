using Luna.Pages.Models.Database.Additional;
using Luna.Pages.Repositories.Repositories.Page.Query;
using Luna.Pages.Services.Queries.Page;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Query.Page;

public class GetWorkspacePageStatisticsQueryHandler : PageQueryHandlerBase, IRequestHandler<GetWorkspacePageStatisticsQuery, PageStatistics>
{
	public GetWorkspacePageStatisticsQueryHandler(IPageQueryRepository pageQueryRepository) : base(pageQueryRepository)
	{
	}

	public async Task<PageStatistics> Handle(GetWorkspacePageStatisticsQuery request, CancellationToken cancellationToken)
	{
		PageStatistics pageStatistics = await PageQueryRepository.GetWorkspacePageStatisticsAsync(request.WorkspaceId, cancellationToken);

		return pageStatistics;
	}
}