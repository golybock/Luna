using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Database.Search;
using Luna.Pages.Models.Domain.Search;
using Luna.Pages.Models.View.Additional;
using Luna.Pages.Repositories.Repositories.Page.Query;
using Luna.Pages.Repositories.Repositories.Search.Query;
using Luna.Pages.Services.Queries.Search;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Query.Search;

public class SearchInBlocksQueryHandler : IRequestHandler<SearchInBlocksQuery, List<SearchPageBlockView>>
{
	private readonly IPageSearchQueryRepository _pageSearchQueryRepository;
	private readonly IPageQueryRepository _pageQueryRepository;
	private ILogger<SearchInBlocksQueryHandler> _logger;

	public SearchInBlocksQueryHandler(
		IPageSearchQueryRepository pageSearchQueryRepository,
		IPageQueryRepository pageQueryRepository,
		ILogger<SearchInBlocksQueryHandler> logger
	)
	{
		_pageSearchQueryRepository = pageSearchQueryRepository;
		_logger = logger;
		_pageQueryRepository = pageQueryRepository;
	}

	public async Task<List<SearchPageBlockView>> Handle(SearchInBlocksQuery request,
		CancellationToken cancellationToken)
	{
		List<PageBlockSearchContent> searchResult = await _pageSearchQueryRepository.SearchInBlocksAsync(
			request.Query,
			request.WorkspaceId,
			request.From,
			request.Size,
			cancellationToken
		);

		List<Guid> pageIds = searchResult
			.Select(item => Guid.Parse(item.PageId))
			.ToList();

		IEnumerable<PageDatabase> pages = await _pageQueryRepository.GetPagesByIdAsync(pageIds, cancellationToken);

		List<SearchPageBlockDomain> result = new List<SearchPageBlockDomain>(searchResult.Count);

		searchResult.ForEach(searchItem =>
		{
			PageDatabase? page = pages.FirstOrDefault(page => page.Id == searchItem.PageId);
			result.Add(SearchPageBlockDomain.FromSearchItem(searchItem, page));
		});

		return result.Select(item => item.ToView()).ToList();
	}
}