using Luna.Pages.Models.Domain.Models;
using MediatR;

namespace Luna.Pages.Services.Queries.Page;

public record SearchPagesByTitleQuery(
	string SearchTerm,
	Guid WorkspaceId,
	int Limit
) : IRequest<IEnumerable<PageDomain>>;