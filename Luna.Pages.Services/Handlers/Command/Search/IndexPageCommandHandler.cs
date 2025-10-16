using Luna.Pages.Models.Database.Search;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Repositories.Repositories.Search.Command;
using Luna.Pages.Services.Commands.Search;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Command.Search;

public class IndexPageCommandHandler : IRequestHandler<IndexPageCommand, bool>
{
	private readonly IPageSearchCommandRepository _pageSearchCommandRepository;
	private readonly ILogger<IndexPageCommandHandler> _logger;

	public IndexPageCommandHandler(IPageSearchCommandRepository pageSearchCommandRepository, ILogger<IndexPageCommandHandler> logger)
	{
		_pageSearchCommandRepository = pageSearchCommandRepository;
		_logger = logger;
	}

	public async Task<bool> Handle(IndexPageCommand request, CancellationToken cancellationToken)
	{
		PageSearchDocument searchDocumentPage = (request.PageVersion ?? new PageVersionDomain())
			.ToSearchDocument(request.PageDomain);

		return await _pageSearchCommandRepository.IndexPageAsync(searchDocumentPage, cancellationToken);
	}
}