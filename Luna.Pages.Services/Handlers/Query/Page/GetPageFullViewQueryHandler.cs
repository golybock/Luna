using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Repositories.Repositories.Page.Query;
using Luna.Pages.Repositories.Repositories.PageVersion;
using Luna.Pages.Repositories.Repositories.PageVersion.Query;
using Luna.Pages.Services.Queries.Page;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Query.Page;

public class GetPageFullViewQueryHandler : PageQueryHandlerBase, IRequestHandler<GetPageFullViewQuery, PageFullDomain?>
{
	private readonly IPageVersionQueryRepository _pageVersionQueryRepository;

	public GetPageFullViewQueryHandler(
		IPageQueryRepository pageQueryRepository,
		IPageVersionQueryRepository pageVersionQueryRepository
	) : base(pageQueryRepository)
	{
		_pageVersionQueryRepository = pageVersionQueryRepository;
	}

	public async Task<PageFullDomain?> Handle(GetPageFullViewQuery request, CancellationToken cancellationToken)
	{
		PageDatabase? page = await PageQueryRepository.GetPageByIdAsync(request.PageId, cancellationToken);

		if (page == null)
		{
			return null;
		}

		PageVersionDatabase? pageVersion = await _pageVersionQueryRepository.GetLatestPageVersionAsync(request.PageId, cancellationToken);

		return new PageFullDomain()
		{
			Page = PageDomain.FromDatabase(page),
			PageVersion = pageVersion == null ? null : PageVersionDomain.FromDatabase(pageVersion)
		};
	}
}