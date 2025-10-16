namespace Luna.Pages.Models.View.Additional;

public class SearchPageBlockView
{
	public string BlockId { get; set; }
	public string PageId { get; set; }
	public string Type { get; set; }
	public string Content { get; set; }
	public LightPageView? Page { get; set; }
}