namespace Luna.Workspaces.Models.Database.Models;

public class WorkspaceUserCache
{
	public Guid UserId { get; set; }
	public Guid WorkspaceId { get; set; }
	public string[] Permissions { get; set; } = null!;
	public Guid? InvitedBy { get; set; }
}