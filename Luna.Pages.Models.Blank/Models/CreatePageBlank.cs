namespace Luna.Pages.Models.Blank.Models;

public class CreatePageBlank
{
	public Guid WorkspaceId { get; set; }
	public Guid? ParentId { get; set; }
	public string Title { get; set; }
	public string? Emoji { get; set; }
	public string? Icon { get; set; }
}