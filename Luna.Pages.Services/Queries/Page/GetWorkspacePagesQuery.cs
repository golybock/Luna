using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Models.View.Additional;
using MediatR;

namespace Luna.Pages.Services.Queries.Page;

public record GetWorkspacePagesQuery(
	Guid WorkspaceId,
	Boolean IncludeArchived
) : IRequest<IEnumerable<PageDomain>>;