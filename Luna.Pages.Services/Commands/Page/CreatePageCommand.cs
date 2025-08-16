using Luna.Pages.Models.Blank.Models;
using MediatR;

namespace Luna.Pages.Services.Commands.Page;

public record CreatePageCommand(
	Guid OperationBy,
	CreatePageBlank CreatePageBlank
) : IRequest<Guid>;