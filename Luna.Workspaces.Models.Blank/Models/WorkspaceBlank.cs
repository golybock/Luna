namespace Luna.Workspaces.Models.Blank.Models;

public class WorkspaceBlank
{
	public string Name { get; set; } = null!;
	public string? Icon { get; set; }
	public string? Description { get; set; }
	public string DefaultPermission { get; set; } = null!;
	public object? Settings { get; set; }
}