using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Database.Search;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Models.View.Additional;
using Luna.Pages.Repositories.Repositories.Page.Query;
using Luna.Pages.Repositories.Repositories.Search.Query;
using Luna.Pages.Services.Queries.Search;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Query.Search;

public class SearchPageQueryHandler : IRequestHandler<SearchPageQuery, List<LightPageView>>
{
	private readonly IPageSearchQueryRepository _pageSearchQueryRepository;
	private readonly IPageQueryRepository _pageQueryRepository;
	private readonly ILogger<SearchPageQueryHandler> _logger;

	public SearchPageQueryHandler(IPageSearchQueryRepository pageSearchQueryRepository, IPageQueryRepository pageQueryRepository, ILogger<SearchPageQueryHandler> logger)
	{
		_pageSearchQueryRepository = pageSearchQueryRepository;
		_pageQueryRepository = pageQueryRepository;
		_logger = logger;
	}

	public async Task<List<LightPageView>> Handle(SearchPageQuery request, CancellationToken cancellationToken)
	{
		List<PageSearchDocument> searchResult = await _pageSearchQueryRepository.SearchAsync(
			request.Query,
			request.WorkspaceId,
			request.From,
			request.Size,
			cancellationToken
		);

		List<Guid> pageIds = searchResult
			.Select(item => Guid.Parse(item.PageId))
			.ToList();

		_logger.LogInformation("PageIds: {PageIds}", string.Join(", ", pageIds));

		IEnumerable<PageDatabase> pages = await _pageQueryRepository.GetPagesByIdAsync(pageIds, cancellationToken);

		return pages
			.Select(PageDomain.FromDatabase)
			.Select(item => item.ToLightPageView())
			.ToList();
	}
}