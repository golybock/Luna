namespace Luna.Pages.Models.View.Models;

public class PageCommentView
{
	public Guid Id { get; set; }
	public Guid PageId { get; set; }
	public string? Content { get; set; }
	public Guid? ParentId { get; set; }
	public Guid? BlockId { get; set; }
	public object? Reactions { get; set; }
	public DateTime? DeletedAt { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}