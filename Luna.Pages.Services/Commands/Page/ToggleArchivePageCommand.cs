using Luna.Pages.Models.Blank.Models;
using MediatR;

namespace Luna.Pages.Services.Commands.Page;

public record ToggleArchivePageCommand(
	Guid OperationBy,
	ToggleArchivePageBlank ToggleArchivePageBlank
) : IRequest<bool>;