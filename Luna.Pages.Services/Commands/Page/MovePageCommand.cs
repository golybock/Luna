using Luna.Pages.Models.Blank.Models;
using MediatR;

namespace Luna.Pages.Services.Commands.Page;

public record MovePageCommand(
	Guid OperationBy,
	MovePageBlank MovePageBlank
) : IRequest<bool>;