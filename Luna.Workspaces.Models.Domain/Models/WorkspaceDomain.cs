namespace Luna.Workspaces.Domain.Models;

public class WorkspaceDomain
{
	public Guid Id { get; set; }
	public string Name { get; set; } = null!;
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public Guid OwnerId { get; set; }
	public string? Icon { get; set; }
	public string? Description { get; set; }
	public string DefaultPermission { get; set; } = null!;
	public object? Settings { get; set; }
	public DateTime? DeletedAt { get; set; }
	public virtual ICollection<WorkspaceUserDomain> Users { get; set; } = new List<WorkspaceUserDomain>();
}