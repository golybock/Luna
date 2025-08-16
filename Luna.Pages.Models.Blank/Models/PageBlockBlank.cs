namespace Luna.Pages.Models.Blank.Models;

public class PageBlockBlank
{
	public string Type { get; set; } = null!;
	public object? Content { get; set; }
	public Guid? ParentId { get; set; }
	public int Index { get; set; }
	public object? Properties { get; set; }
}