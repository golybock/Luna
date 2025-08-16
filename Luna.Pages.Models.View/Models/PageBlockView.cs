namespace Luna.Pages.Models.View.Models;

public class PageBlockView
{
	public Guid Id { get; set; }
	public string Type { get; set; } = null!;
	public object? Content { get; set; }
	public Guid? ParentId { get; set; }
	public int Index { get; set; }
	public object? Properties { get; set; }
}