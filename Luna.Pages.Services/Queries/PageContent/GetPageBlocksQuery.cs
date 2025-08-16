using Luna.Pages.Models.Domain.Models;
using MediatR;

namespace Luna.Pages.Services.Queries.PageContent;

public record GetPageBlocksQuery(
	Guid PageId
) : IRequest<IEnumerable<PageBlockDomain>>;