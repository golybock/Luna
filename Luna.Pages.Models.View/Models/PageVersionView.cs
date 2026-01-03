namespace Luna.Pages.Models.View.Models;

public class PageVersionView
{
	public Guid Id { get; set; }
	public Guid PageId { get; set; }
	public int Version  { get; set; }
	public object? Document { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public Guid CreatedBy { get; set; }
	public string? ChangeDescription { get; set; }
}