using Luna.Pages.Repositories.Repositories.Page.Query;
using Luna.Pages.Services.Queries.Page;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Query.Page;

public class PageExistsQueryHandler: PageQueryHandlerBase, IRequestHandler<PageExistsQuery, bool>
{
	public PageExistsQueryHandler(IPageQueryRepository pageQueryRepository) : base(pageQueryRepository)
	{
	}

	public async Task<bool> Handle(PageExistsQuery request, CancellationToken cancellationToken)
	{
		return await PageQueryRepository.PageExistsAsync(request.PageId, cancellationToken);
	}
}