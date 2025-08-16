using MediatR;

namespace Luna.Pages.Services.Commands.Page;

public record DeletePageCommand(
	Guid PageId,
	Guid DeletedBy
) : IRequest<bool>;