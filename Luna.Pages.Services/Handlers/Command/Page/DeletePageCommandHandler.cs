using Luna.Pages.Repositories.Repositories.Page.Command;
using Luna.Pages.Services.Commands.Page;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Command.Page;

public class DeletePageCommandHandler : IRequestHandler<DeletePageCommand, bool>
{
	private readonly IPageCommandRepository _pageCommandRepository;
	private readonly ILogger<DeletePageCommandHandler> _logger;

	public DeletePageCommandHandler(
		IPageCommandRepository commandRepository,
		ILogger<DeletePageCommandHandler> logger)
	{
		_pageCommandRepository = commandRepository;
		_logger = logger;
	}

	public async Task<bool> Handle(DeletePageCommand request, CancellationToken cancellationToken)
	{
		return await _pageCommandRepository.DeletePageAsync(request.PageId, request.DeletedBy, cancellationToken);
	}
}