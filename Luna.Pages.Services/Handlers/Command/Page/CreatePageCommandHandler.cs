using System.Text.Json;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Repositories.Repositories.Page.Command;
using Luna.Pages.Repositories.Repositories.PageVersion.Command;
using Luna.Pages.Services.Commands.Page;
using MediatR;
using MongoDB.Bson;

namespace Luna.Pages.Services.Handlers.Command.Page;

public class CreatePageCommandHandler : PageCommandHandlerBase, IRequestHandler<CreatePageCommand, Guid>
{
	private readonly IPageVersionCommandRepository _pageVersionCommandRepository;

	public CreatePageCommandHandler(
		IPageCommandRepository pageCommandRepository,
		IPageVersionCommandRepository pageVersionCommandRepository
	) : base(pageCommandRepository)
	{
		_pageVersionCommandRepository = pageVersionCommandRepository;
	}

	public async Task<Guid> Handle(CreatePageCommand request, CancellationToken cancellationToken)
	{
		Guid pageId = Guid.NewGuid();
		Guid versionId = Guid.NewGuid();

		PageDomain pageDomain = PageDomain.FromBlank(pageId, request.OperationBy, request.CreatePageBlank);
		PageVersionDomain pageVersionDomain = PageVersionDomain.CreateInitial(versionId, pageId, request.OperationBy);

		Console.WriteLine("Insert page:");
		Console.WriteLine(pageDomain.ToDatabase().ToBsonDocument().ToJson());

		await PageCommandRepository.CreatePageAsync(pageDomain.ToDatabase(), cancellationToken);
		await _pageVersionCommandRepository.CreatePageVersionAsync(pageVersionDomain.ToDatabase(), cancellationToken);

		return pageId;
	}
}