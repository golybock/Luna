namespace Luna.Tools.SharedModels.Models.Kafka;

public class KafkaSettings
{
	public string BootstrapServers { get; set; } = null!;
	public string ClientId { get; set; } = null!;
	public string PermissionEventsTopic { get; set; } = null!;
	public string ConsumerGroupId  { get; set; } = null!;
}