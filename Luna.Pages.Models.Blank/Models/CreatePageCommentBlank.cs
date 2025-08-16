namespace Luna.Pages.Models.Blank.Models;

public class CreatePageCommentBlank
{
	public Guid PageId { get; set; }
	public string? Content { get; set; }
	public Guid? ParentId { get; set; }
	public Guid? BlockId { get; set; }
	public object? Reactions { get; set; }
}