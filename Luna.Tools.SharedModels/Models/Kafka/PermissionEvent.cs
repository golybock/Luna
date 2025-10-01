namespace Luna.Tools.SharedModels.Models.Kafka;

public class PermissionEvent
{
	public PermissionEventType EventType { get; set; }
	public DateTime Timestamp { get; set; }
	public object Data { get; set; } = null!;
}