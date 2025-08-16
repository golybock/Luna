using Luna.Tools.SharedModels.Models;

namespace Luna.Users.Models.View.Models;

public class ReminderView
{
	public Guid Id { get; set; }
	public Guid EntityId { get; set; }
	public int EntityType { get; set; }
	public DateTime DueAt { get; set; }
	public string Title { get; set; } = null!;
	public string? Description { get; set; }
	public ReminderStatus Status { get; set; }
	public string? RepeatRule { get; set; }
}