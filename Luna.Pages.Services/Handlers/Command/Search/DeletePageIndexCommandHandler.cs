using Luna.Pages.Repositories.Repositories.Search.Command;
using Luna.Pages.Services.Commands.Search;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Command.Search;

public class DeletePageIndexCommandHandler : IRequestHandler<DeletePageIndexCommand, bool>
{
	private readonly IPageSearchCommandRepository _pageSearchCommandRepository;
	private readonly ILogger<DeletePageIndexCommandHandler> _logger;

	public DeletePageIndexCommandHandler(IPageSearchCommandRepository pageSearchCommandRepository, ILogger<DeletePageIndexCommandHandler> logger)
	{
		_logger = logger;
		_pageSearchCommandRepository = pageSearchCommandRepository;
	}

	public async Task<bool> Handle(DeletePageIndexCommand request, CancellationToken cancellationToken)
	{
		return await _pageSearchCommandRepository.DeletePageAsync(request.PageId, cancellationToken);
	}
}