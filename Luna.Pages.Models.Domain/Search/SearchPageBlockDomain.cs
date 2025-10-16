using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Database.Search;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Models.View.Additional;

namespace Luna.Pages.Models.Domain.Search;

public class SearchPageBlockDomain
{
	public string BlockId { get; set; }
	public string PageId { get; set; }
	public string Type { get; set; }
	public string Content { get; set; }

	public PageDomain? Page { get; set; }

	public static SearchPageBlockDomain FromSearchItem(PageBlockSearchContent searchContent, PageDatabase? pageDatabase)
	{
		return new SearchPageBlockDomain
		{
			BlockId = searchContent.BlockId,
			PageId = searchContent.PageId,
			Type = searchContent.Type,
			Content = searchContent.Content,
			Page = pageDatabase != null ? PageDomain.FromDatabase(pageDatabase) : null
		};
	}

	public SearchPageBlockView ToView()
	{
		return new SearchPageBlockView()
		{
			BlockId = BlockId,
			PageId = PageId,
			Type = Type,
			Content = Content,
			Page = Page?.ToLightPageView()
		};
	}
}