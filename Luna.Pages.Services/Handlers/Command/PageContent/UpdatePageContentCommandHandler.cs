using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Repositories.Repositories.PageVersion;
using Luna.Pages.Repositories.Repositories.PageVersion.Command;
using Luna.Pages.Services.Commands.PageContent;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Luna.Pages.Services.Handlers.Command.PageContent;

public class UpdatePageContentCommandHandler : IRequestHandler<UpdatePageContentCommand, bool>
{
	private readonly IPageVersionCommandRepository _pageVersionCommandRepository;
	private readonly ILogger<UpdatePageContentCommandHandler> _logger;

	public UpdatePageContentCommandHandler(
		IPageVersionCommandRepository pageVersionCommandRepository,
		ILogger<UpdatePageContentCommandHandler> logger
	)
	{
		_pageVersionCommandRepository = pageVersionCommandRepository;
		_logger = logger;
	}

	public async Task<bool> Handle(UpdatePageContentCommand request, CancellationToken cancellationToken)
	{
		Guid id = Guid.NewGuid();

		PageVersionDomain pageVersionDomain = PageVersionDomain.CreateFromBlank(id, request.PageId, request.OperationBy, request.UpdatePageContentBlank);

		PageVersionDatabase databasePageVersion = pageVersionDomain.ToDatabase();

		Console.WriteLine(databasePageVersion.ToJson());

		return await _pageVersionCommandRepository.CreatePageVersionAsync(databasePageVersion, cancellationToken);
	}
}