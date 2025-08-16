using MediatR;

namespace Luna.Pages.Services.Queries.Page;

public record PageExistsQuery(
	Guid PageId
) : IRequest<bool>;