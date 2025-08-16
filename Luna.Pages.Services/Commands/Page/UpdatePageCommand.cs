using Luna.Pages.Models.Blank.Models;
using MediatR;

namespace Luna.Pages.Services.Commands.Page;

public record UpdatePageCommand(
	Guid PageId,
	PatchPageBlank PatchPageBlank
) : IRequest<bool>;