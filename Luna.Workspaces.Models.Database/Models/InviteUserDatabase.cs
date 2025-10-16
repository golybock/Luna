namespace Luna.Workspaces.Models.Database.Models;

public class InviteUserDatabase
{
	public string Email { get; set; } = null!;
	public Guid WorkspaceId { get; set; }
	public string[] Permissions { get; set; } = null!;
	public Guid? InvitedBy { get; set; }
}