using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Repositories.Repositories.Page.Query;
using Luna.Pages.Services.Queries.Page;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Query.Page;

public class GetPageByIdQueryHandler : PageQueryHandlerBase, IRequestHandler<GetPageByIdQuery, PageDomain?>
{
	public GetPageByIdQueryHandler(IPageQueryRepository pageQueryRepository) : base(pageQueryRepository)
	{
	}

	public async Task<PageDomain?> Handle(GetPageByIdQuery request, CancellationToken cancellationToken)
	{
		PageDatabase? page = await PageQueryRepository.GetPageByIdAsync(request.PageId, cancellationToken);

		return page == null ? null : PageDomain.FromDatabase(page);
	}
}