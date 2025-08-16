using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Repositories.Repositories.Page.Query;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Handlers.Query.Page;

public abstract class PageQueryHandlerBase
{
	protected readonly IPageQueryRepository PageQueryRepository;

	protected PageQueryHandlerBase(IPageQueryRepository pageQueryRepository)
	{
		PageQueryRepository = pageQueryRepository;
	}

	protected IEnumerable<PageDomain> BuildHierarchy(IEnumerable<PageDatabase> allPages)
	{
		Dictionary<string, PageDomain> pageDict = allPages.ToDictionary(p => p.Id, PageDomain.FromDatabase);
		List<PageDomain> rootPages = new List<PageDomain>();

		foreach (PageDomain page in pageDict.Values)
		{
			if (page.ParentId == null)
			{
				rootPages.Add(page);
			}
			else if (pageDict.TryGetValue(page.ParentId.Value.ToString(), out PageDomain? parent))
			{
				parent.ChildPages.Add(page);
			}
		}

		SortChildrenRecursively(rootPages);

		return rootPages.OrderBy(p => p.Index ?? 0);
	}

	private void SortChildrenRecursively(IEnumerable<PageDomain> pages)
	{
		foreach (PageDomain page in pages)
		{
			page.ChildPages = page.ChildPages.OrderBy(p => p.Index ?? 0).ToList();
			if (page.ChildPages.Any())
			{
				SortChildrenRecursively(page.ChildPages);
			}
		}
	}
}