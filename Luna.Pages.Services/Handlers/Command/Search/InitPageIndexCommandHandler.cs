using Luna.Pages.Repositories.Repositories.Search.Command;
using Luna.Pages.Services.Commands.Search;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Command.Search;

public class InitPageIndexCommandHandler : IRequestHandler<InitPageIndexCommand>
{
	private readonly IPageSearchCommandRepository _pageSearchCommandRepository;
	private readonly ILogger<InitPageIndexCommandHandler> _logger;

	public InitPageIndexCommandHandler(IPageSearchCommandRepository pageSearchCommandRepository, ILogger<InitPageIndexCommandHandler> logger)
	{
		_pageSearchCommandRepository = pageSearchCommandRepository;
		_logger = logger;
	}

	public async Task Handle(InitPageIndexCommand request, CancellationToken cancellationToken)
	{
		await _pageSearchCommandRepository.CreateIndexAsync(cancellationToken);
	}
}