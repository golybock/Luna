using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Domain.Models;
using MediatR;

namespace Luna.Pages.Services.Queries.Page;

public record GetPageByIdQuery(
	Guid PageId
) : IRequest<PageDomain?>;