using System.Text.Json;
using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Repositories.Repositories.PageVersion.Query;
using Luna.Pages.Services.Queries.PageContent;
using MediatR;

namespace Luna.Pages.Services.Handlers.Query.PageContent;

public class GetPageBlocksQueryHandler : IRequestHandler<GetPageBlocksQuery, IEnumerable<PageBlockDomain>>
{
	private readonly IPageVersionQueryRepository _pageVersionQueryRepository;

	public GetPageBlocksQueryHandler(IPageVersionQueryRepository pageVersionRepository)
	{
		_pageVersionQueryRepository = pageVersionRepository;
	}

	public async Task<IEnumerable<PageBlockDomain>> Handle(GetPageBlocksQuery request, CancellationToken cancellationToken)
	{
		PageVersionDatabase? lastVersion = await _pageVersionQueryRepository.GetLatestPageVersionAsync(request.PageId, cancellationToken);

		if (lastVersion == null)
		{
			return [];
		}

		return PageVersionDomain.FromDatabase(lastVersion).Content ?? [];
	}
}