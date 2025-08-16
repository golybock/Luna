namespace Luna.Users.Models.Domain.Models;

public class UserDomain
{
	public Guid Id { get; set; }

	public string Username { get; set; } = null!;

	public string? DisplayName { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }

	public string? Image { get; set; }

	public string? Bio { get; set; }

	public DateTime LastActive { get; set; }

	public virtual ICollection<BookmarkDomain> Bookmarks { get; set; } = new List<BookmarkDomain>();

	public virtual ICollection<ReminderDomain> Reminders { get; set; } = new List<ReminderDomain>();

	public virtual ICollection<UserSettingsDomain> UserSettings { get; set; } = new List<UserSettingsDomain>();
}