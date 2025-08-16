namespace Luna.Pages.Models.Blank.Models;

public class PatchPageCommentBlank
{
	public string? Content { get; set; }
	public Guid? ParentId { get; set; }
	public Guid? BlockId { get; set; }
	public object? Reactions { get; set; }
}