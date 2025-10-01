namespace Luna.Pages.Models.Database.WorkspaceUser;

public class WorkspaceUserDatabase
{
	public Guid Id { get; set; }

	public Guid UserId { get; set; }

	public Guid WorkspaceId { get; set; }

	public List<string> Permissions { get; set; } = null!;

	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }
}