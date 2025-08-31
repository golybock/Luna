using System.Text.Json;
using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Repositories.Repositories.PageVersion;
using Luna.Pages.Repositories.Repositories.PageVersion.Query;
using Luna.Pages.Services.Queries.PageContent;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Luna.Pages.Services.Handlers.Query.PageContent;

public class GetPageBlocksQueryHandler : IRequestHandler<GetPageBlocksQuery, IEnumerable<PageBlockDomain>>
{
	private readonly IPageVersionQueryRepository _pageVersionQueryRepository;
	private readonly ILogger _logger;

	public GetPageBlocksQueryHandler(IPageVersionQueryRepository pageVersionRepository,
		ILogger logger)
	{
		_pageVersionQueryRepository = pageVersionRepository;
		_logger = logger;
	}

	public async Task<IEnumerable<PageBlockDomain>> Handle(GetPageBlocksQuery request, CancellationToken cancellationToken)
	{
		PageVersionDatabase? lastVersion =
			await _pageVersionQueryRepository.GetLatestPageVersionAsync(request.PageId, cancellationToken);

		if (lastVersion == null)
		{
			return [];
		}

		IEnumerable<PageBlockDatabase> blocksDatabase = JsonSerializer.Deserialize<IEnumerable<PageBlockDatabase>>(lastVersion.Content.ToJson())!;

		return blocksDatabase.Select(PageBlockDomain.FromDatabase);
	}
}