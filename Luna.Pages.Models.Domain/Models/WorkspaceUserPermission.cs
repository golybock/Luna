namespace Luna.Pages.Models.Domain.Models;

public class WorkspaceUserPermission
{
	public Guid UserId { get; set; }
	public Guid WorkspaceId { get; set; }
	public string[] Permissions { get; set; } = null!;
}