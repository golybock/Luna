using System.Text.Json.Serialization;

namespace Luna.Workspaces.Domain.Models;

public class WorkspaceUserPermission
{
	[JsonPropertyName("userId")]
	public Guid UserId { get; set; }

	[JsonPropertyName("workspaceId")]
	public Guid WorkspaceId { get; set; }

	[JsonPropertyName("permissions")]
	public string[] Permissions { get; set; } = null!;
}