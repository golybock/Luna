namespace Luna.Tools.SharedModels.Models.Outbox;

public static class OutboxMessageStatus
{
	public const int Pending = 0;
	public const int Processing = 1;
	public const int Sent = 2;
	public const int Failed = 3;
}
