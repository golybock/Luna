namespace Luna.Pages.Models.View.Models;

public class PageFullView
{
	public PageView Page { get; set; } = null!;
	public PageVersionView? PageVersionView { get; set; }
}