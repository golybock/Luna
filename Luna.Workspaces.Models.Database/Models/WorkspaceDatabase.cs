namespace Luna.Workspaces.Models.Database.Models;

public class WorkspaceDatabase
{
	public Guid Id { get; set; }
	public string Name { get; set; } = null!;
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public Guid OwnerId { get; set; }
	public string? Icon { get; set; }
	public string? Description { get; set; }
	public string Visibility { get; set; } = null!;
	public string DefaultPermission { get; set; } = null!;
	public object? Settings { get; set; }
	public DateTime? DeletedAt { get; set; }
	public virtual ICollection<WorkspaceUserDatabase> WorkspaceUsers { get; set; } = new List<WorkspaceUserDatabase>();
}