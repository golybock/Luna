namespace Luna.Workspaces.Models.View.Models;

public class WorkspaceUserView
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public Guid WorkspaceId { get; set; }
	public string[] Permissions { get; set; } = null!;
	public Guid? InvitedBy { get; set; }
	public DateTime? AcceptedAt { get; set; }
}