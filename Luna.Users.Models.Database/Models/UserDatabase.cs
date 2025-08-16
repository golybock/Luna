namespace Luna.Users.Models.Database.Models;

public class UserDatabase
{
	public Guid Id { get; set; }

	public string Username { get; set; } = null!;

	public string? DisplayName { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }

	public string? Image { get; set; }

	public string? Bio { get; set; }

	public DateTime LastActive { get; set; }

	public virtual ICollection<BookmarkDatabase> Bookmarks { get; set; } = new List<BookmarkDatabase>();

	public virtual ICollection<ReminderDatabase> Reminders { get; set; } = new List<ReminderDatabase>();

	public virtual ICollection<UserSettingsDatabase> UserSettings { get; set; } = new List<UserSettingsDatabase>();
}