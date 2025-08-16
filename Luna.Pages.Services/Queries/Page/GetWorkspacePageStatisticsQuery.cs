using Luna.Pages.Models.Database.Additional;
using MediatR;

namespace Luna.Pages.Services.Queries.Page;

public record GetWorkspacePageStatisticsQuery(
	Guid WorkspaceId
) : IRequest<PageStatistics>;