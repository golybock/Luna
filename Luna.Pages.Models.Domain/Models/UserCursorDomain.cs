using Luna.Pages.Models.Blank.Models;
using Luna.Pages.Models.View.Additional;

namespace Luna.Pages.Models.Domain.Models;

public class UserCursorDomain
{
	public string BlockId { get; set; } = null!;
	public int Position { get; set; }
	public string UserId { get; set; } = null!;
	public string? UserDisplayName { get; set; }

	public static UserCursorDomain FromBlank(UserCursorBlank blank, string userId, string? userDisplayName)
	{
		return new UserCursorDomain
		{
			BlockId = blank.BlockId,
			Position = blank.Position,
			UserId = userId,
			UserDisplayName = userDisplayName
		};
	}

	public UserCursorView ToView()
	{
		return new UserCursorView()
		{
			BlockId = BlockId,
			Position = Position,
			UserId = UserId,
			UserDisplayName = UserDisplayName
		};
	}
}