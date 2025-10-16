namespace Luna.Workspaces.Models.View.Models;

public class WorkspaceDetailedView
{
	public Guid Id { get; set; }
	public string Name { get; set; } = null!;
	public Guid OwnerId { get; set; }
	public string? Icon { get; set; }
	public string? Description { get; set; }
	public string DefaultPermission { get; set; } = null!;
	public object? Settings { get; set; }
	public DateTime? DeletedAt { get; set; }
	public IEnumerable<WorkspaceUserDetailedView> Users { get; set; } = new List<WorkspaceUserDetailedView>();
}