using Luna.Pages.Models.Database.Models;
using Luna.Pages.Repositories.Repositories.Page.Command;
using Luna.Pages.Services.Commands.Page;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Command.Page;

public class ToggleArchivePageCommandHandler : PageCommandHandlerBase, IRequestHandler<ToggleArchivePageCommand, bool>
{
	public ToggleArchivePageCommandHandler(IPageCommandRepository pageCommandRepository)
		: base(pageCommandRepository) { }

	public async Task<bool> Handle(ToggleArchivePageCommand request, CancellationToken cancellationToken)
	{
		Dictionary<string, object?> updates = new()
		{
			{nameof(PageDatabase.ArchivedAt), request.ToggleArchivePageBlank.Archived ? DateTime.UtcNow : null},
		};

		return await PageCommandRepository.PatchPageAsync(request.ToggleArchivePageBlank.PageId, updates, cancellationToken);
	}
}