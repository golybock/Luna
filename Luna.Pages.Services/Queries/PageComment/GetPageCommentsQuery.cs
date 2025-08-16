using Luna.Pages.Models.Domain.Models;
using MediatR;

namespace Luna.Pages.Services.Queries.PageComment;

public record GetPageCommentsQuery(
	Guid PageId
) : IRequest<IEnumerable<PageCommentDomain>>;