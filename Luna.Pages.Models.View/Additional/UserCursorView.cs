namespace Luna.Pages.Models.View.Additional;

public class UserCursorView
{
	public string BlockId { get; set; } = null!;
	public int Position { get; set; }
	public string UserId { get; set; } = null!;
	public string? UserDisplayName { get; set; }
}