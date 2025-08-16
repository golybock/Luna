using Luna.Pages.Models.View.Models;

namespace Luna.Pages.Models.Domain.Models;

public class PageFullDomain
{
	public PageDomain Page { get; set; } = null!;
	public PageVersionDomain? PageVersion { get; set; } = null!;

	public PageFullView ToView()
	{
		return new PageFullView()
		{
			Page = Page.ToView(),
			PageVersionView = PageVersion?.ToView()
		};
	}
}