using Luna.Pages.Models.Blank.Models;
using MediatR;

namespace Luna.Pages.Services.Commands.Page;

public record TogglePinPageCommand(
	Guid OperationBy,
	TogglePinPageBlank TogglePinPageBlank
) : IRequest<bool>;