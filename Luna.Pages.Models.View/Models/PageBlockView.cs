namespace Luna.Pages.Models.View.Models;

public class PageBlockView
{
	public string Id { get; set; } = null!;
	public string Type { get; set; } = null!;
	public object? Data { get; set; }
}