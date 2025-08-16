namespace Luna.Users.Models.Domain.Models;

public class ReminderDomain
{
	public Guid Id { get; set; }

	public Guid UserId { get; set; }

	public Guid EntityId { get; set; }

	public int EntityType { get; set; }

	public DateTime DueAt { get; set; }

	public string Title { get; set; } = null!;

	public string? Description { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }

	public int Status { get; set; }

	public bool NotificationSent { get; set; }

	public string? RepeatRule { get; set; }
}