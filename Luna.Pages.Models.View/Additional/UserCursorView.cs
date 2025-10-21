using Luna.Users.Models.View.Models;

namespace Luna.Pages.Models.View.Additional;

public class UserCursorView
{
	public string BlockId { get; set; } = null!;
	public int Position { get; set; }
	public Guid UserId { get; set; }
	public UserView? User { get; set; }
}