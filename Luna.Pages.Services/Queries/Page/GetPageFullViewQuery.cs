using Luna.Pages.Models.Domain.Models;
using MediatR;

namespace Luna.Pages.Services.Queries.Page;

public record GetPageFullViewQuery(
	Guid PageId
) : IRequest<PageFullDomain?>;