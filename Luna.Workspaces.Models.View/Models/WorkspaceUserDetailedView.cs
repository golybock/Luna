using Luna.Users.Models.View.Models;

namespace Luna.Workspaces.Models.View.Models;

public class WorkspaceUserDetailedView
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public Guid WorkspaceId { get; set; }
	public string[] Permissions { get; set; } = null!;
	public Guid? InvitedBy { get; set; }
	public DateTime? AcceptedAt { get; set; }
	public UserView? User { get; set; }
}