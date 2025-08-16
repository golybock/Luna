using Luna.Pages.Models.Database.Models;
using Luna.Pages.Repositories.Repositories.Page.Command;
using Luna.Pages.Services.Commands.Page;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Command.Page;

public class TogglePinPageCommandHandler : PageCommandHandlerBase, IRequestHandler<TogglePinPageCommand, bool>
{
	public TogglePinPageCommandHandler(IPageCommandRepository pageCommandRepository)
		: base(pageCommandRepository) { }

	public async Task<bool> Handle(TogglePinPageCommand request, CancellationToken cancellationToken)
	{
		Dictionary<string, object?> updates = new()
		{
			{nameof(PageDatabase.Pinned), request.TogglePinPageBlank.IsPinned},
		};

		return await PageCommandRepository.PatchPageAsync(request.TogglePinPageBlank.PageId, updates, cancellationToken);
	}
}