using Luna.Pages.Models.Blank.Models;
using Luna.Pages.Models.View.Additional;
using Luna.Users.Models.Domain.Models;

namespace Luna.Pages.Models.Domain.Models;

public class UserCursorDomain
{
	public string BlockId { get; set; } = null!;
	public int Position { get; set; }
	public Guid UserId { get; set; }
	public UserDomain? User { get; set; }

	public static UserCursorDomain FromBlank(UserCursorBlank blank, UserDomain? user, Guid userId)
	{
		return new UserCursorDomain
		{
			BlockId = blank.BlockId,
			Position = blank.Position,
			UserId = userId,
			User = user
		};
	}

	public UserCursorView ToView()
	{
		return new UserCursorView()
		{
			BlockId = BlockId,
			Position = Position,
			UserId = UserId,
			User = User?.ToView()
		};
	}
}