namespace Luna.Users.Models.View.Models;

public class UserFullView : UserView
{
	public virtual ICollection<BookmarkView> Bookmarks { get; set; } = new List<BookmarkView>();

	public virtual ICollection<ReminderView> Reminders { get; set; } = new List<ReminderView>();

	public virtual ICollection<UserSettingsView> UserSettings { get; set; } = new List<UserSettingsView>();
}