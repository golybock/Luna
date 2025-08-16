namespace Luna.Workspaces.Models.Blank.Models;

public class WorkspaceUserBlank
{
	public Guid UserId { get; set; }
	public Guid WorkspaceId { get; set; }
	public string[] Roles { get; set; } = null!;
	public string[] Permissions { get; set; } = null!;
}