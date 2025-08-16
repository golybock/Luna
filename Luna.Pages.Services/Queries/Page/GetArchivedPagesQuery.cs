using Luna.Pages.Models.Domain.Models;
using MediatR;

namespace Luna.Pages.Services.Queries.Page;

public record GetArchivedPagesQuery(
	Guid WorkspaceId
) : IRequest<IEnumerable<PageDomain>>;