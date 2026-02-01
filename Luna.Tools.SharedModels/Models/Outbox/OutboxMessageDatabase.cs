namespace Luna.Tools.SharedModels.Models.Outbox;

public class OutboxMessageDatabase
{
	public Guid Id { get; set; }

	public string Type { get; set; } = null!;

	public string Payload { get; set; } = null!;

	public int Status { get; set; }

	public int Attempts { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime? ProcessedAt { get; set; }

	public string? LastError { get; set; }

	public DateTime? LockedUntil { get; set; }
}
