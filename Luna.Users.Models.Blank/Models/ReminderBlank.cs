namespace Luna.Users.Models.Blank.Models;

public class ReminderBlank
{
	public Guid EntityId { get; set; }

	public DateTime DueAt { get; set; }

	public string Title { get; set; } = null!;

	public string? Description { get; set; }

	public int Status { get; set; }

	public string? RepeatRule { get; set; }
}