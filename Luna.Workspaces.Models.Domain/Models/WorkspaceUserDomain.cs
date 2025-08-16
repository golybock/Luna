namespace Luna.Workspaces.Domain.Models;

public class WorkspaceUserDomain
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public Guid WorkspaceId { get; set; }
	public string[] Roles { get; set; } = null!;
	public string[] Permissions { get; set; } = null!;
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public Guid? InvitedBy { get; set; }
	public DateTime? AcceptedAt { get; set; }
}