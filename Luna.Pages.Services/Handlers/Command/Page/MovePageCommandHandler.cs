using Luna.Pages.Models.Database.Models;
using Luna.Pages.Repositories.Repositories.Page.Command;
using Luna.Pages.Services.Commands.Page;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Command.Page;

public class MovePageCommandHandler : PageCommandHandlerBase, IRequestHandler<MovePageCommand, bool>
{
	public MovePageCommandHandler(IPageCommandRepository pageCommandRepository)
		: base(pageCommandRepository) { }

	public async Task<bool> Handle(MovePageCommand request, CancellationToken cancellationToken)
	{
		Dictionary<string, object?> updates = new Dictionary<string, object?>
		{
			{nameof(PageDatabase.Index), request.MovePageBlank.NewIndex},
			{nameof(PageDatabase.ParentId), request.MovePageBlank.NewParentId}
		};

		return await PageCommandRepository.PatchPageAsync(request.MovePageBlank.PageId, updates, cancellationToken);
	}
}