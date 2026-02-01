using Luna.Tools.SharedModels.Models.Kafka;

namespace Luna.Workspaces.Domain.Models;

public class PermissionEventOutboxPayload
{
	public string Key { get; set; } = null!;

	public PermissionEvent Event { get; set; } = null!;
}
