using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.Database.Models;
using Luna.Users.Models.View.Models;

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

	public static UserDomain FromBLank(UserBlank userBlank)
	{
		return new UserDomain()
		{
			Username = userBlank.Username!,
			DisplayName = userBlank.DisplayName,
			Image = userBlank.Image,
			Bio = userBlank.Bio
		};
	}

	public static UserDomain FromBLankAndDatabase(UserBlank userBlank, UserDatabase userDatabase)
	{
		UserDomain userDomain = UserDomain.FromDatabase(userDatabase);

		userDomain.Username = userBlank.Username ?? userDatabase.Username;
		userDomain.DisplayName = userBlank.DisplayName ?? userDatabase.DisplayName;
		userDomain.Bio = userBlank.Bio ?? userDatabase.Bio;
		userDomain.Image = userBlank.Image ?? userDatabase.Image;

		return userDomain;
	}

	public static UserDomain FromDatabase(UserDatabase userDatabase)
	{
		return new UserDomain()
		{
			Id = userDatabase.Id,
			Username = userDatabase.Username,
			DisplayName = userDatabase.DisplayName,
			CreatedAt = userDatabase.CreatedAt,
			UpdatedAt = userDatabase.UpdatedAt,
			Image = userDatabase.Image,
			Bio = userDatabase.Bio,
			LastActive = userDatabase.LastActive,
			Bookmarks = userDatabase.Bookmarks.Select(BookmarkDomain.FromDatabase).ToList(),
			Reminders = userDatabase.Reminders.Select(ReminderDomain.FromDatabase).ToList(),
			UserSettings = userDatabase.UserSettings.Select(UserSettingsDomain.FromDatabase).ToList()
		};
	}

	public UserView ToView()
	{
		return new UserView()
		{
			Id = Id,
			Username = Username,
			DisplayName = DisplayName,
			Image = Image,
			Bio = Bio,
			LastActive = LastActive
		};
	}

	public UserDatabase ToDatabase()
	{
		return new UserDatabase()
		{
			Id = Id,
			Username = Username,
			DisplayName = DisplayName,
			CreatedAt = CreatedAt,
			UpdatedAt = UpdatedAt,
			Image = Image,
			Bio = Bio,
			LastActive = LastActive,
			Bookmarks = Bookmarks.Select(b => b.ToDatabase()).ToList(),
			Reminders = Reminders.Select(r => r.ToDatabase()).ToList(),
			UserSettings = UserSettings.Select(us => us.ToDatabase()).ToList()
		};
	}

	public UserFullView ToFullView()
	{
		return new UserFullView()
		{
			Id = Id,
			Username = Username,
			DisplayName = DisplayName,
			Image = Image,
			Bio = Bio,
			LastActive = LastActive,
			Bookmarks = Bookmarks.Select(b => b.ToView()).ToList(),
			Reminders = Reminders.Select(r => r.ToView()).ToList(),
			UserSettings = UserSettings.Select(us => us.ToView()).ToList()
		};
	}
}