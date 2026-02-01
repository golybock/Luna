namespace Luna.Pages.Models.Database.Search;

public class PageSearchDocument
{
	public string PageId { get; set; }
	public string Title { get; set; }
	public string? Description { get; set; }
	public string Content { get; set; } = string.Empty;
	public string WorkspaceId { get; set; }
	public DateTime UpdatedAt { get; set; }
	public IEnumerable<PageBlockSearchContent> Blocks { get; set; } = new List<PageBlockSearchContent>();
}