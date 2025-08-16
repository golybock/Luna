using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.Database.Models;
using Luna.Users.Models.Domain.Models;
using Luna.Users.Models.View.Models;
using Luna.Users.Services.Extensions;

namespace Luna.Users.Models.Extensions.Extensions;

public static class UserExtensions
{
	public static UserView ToView(this UserDatabase user)
	{
		return new UserView()
		{
			Id = user.Id,
			Username = user.Username,
			DisplayName = user.DisplayName,
			Image = user.Image,
			Bio = user.Bio,
			LastActive = user.LastActive
		};
	}

	public static UserView ToView(this UserDomain user)
	{
		return new UserView()
		{
			Id = user.Id,
			Username = user.Username,
			DisplayName = user.DisplayName,
			Image = user.Image,
			Bio = user.Bio,
			LastActive = user.LastActive
		};
	}

	public static UserFullView ToFullView(this UserDatabase user)
	{
		return new UserFullView()
		{
			Id = user.Id,
			Username = user.Username,
			DisplayName = user.DisplayName,
			Image = user.Image,
			Bio = user.Bio,
			LastActive = user.LastActive,
			Bookmarks = user.Bookmarks.Select(b => b.ToView()).ToList(),
			Reminders = user.Reminders.Select(r => r.ToView()).ToList(),
			UserSettings = user.UserSettings.Select(us => us.ToView()).ToList()
		};
	}

	public static UserFullView ToFullView(this UserDomain user)
	{
		return new UserFullView()
		{
			Id = user.Id,
			Username = user.Username,
			DisplayName = user.DisplayName,
			Image = user.Image,
			Bio = user.Bio,
			LastActive = user.LastActive,
			Bookmarks = user.Bookmarks.Select(b => b.ToView()).ToList(),
			Reminders = user.Reminders.Select(r => r.ToView()).ToList(),
			UserSettings = user.UserSettings.Select(us => us.ToView()).ToList()
		};
	}

	public static UserDomain ToDomain(this UserDatabase user)
	{
		return new UserDomain()
		{
			Id = user.Id,
			Username = user.Username,
			DisplayName = user.DisplayName,
			CreatedAt = user.CreatedAt,
			UpdatedAt = user.UpdatedAt,
			Image = user.Image,
			Bio = user.Bio,
			LastActive = user.LastActive,
			Bookmarks = user.Bookmarks.Select(b => b.ToDomain()).ToList(),
			Reminders = user.Reminders.Select(r => r.ToDomain()).ToList(),
			UserSettings = user.UserSettings.Select(us => us.ToDomain()).ToList()
		};
	}

	// создание нового пользователя
	public static UserDomain ToDomain(this UserBlank user)
	{
		return new UserDomain()
		{
			Username = user.Username!,
			DisplayName = user.DisplayName,
			Image = user.Image,
			Bio = user.Bio
		};
	}

	// обновление старого пользователя
	public static UserDomain ToDomain(this UserBlank user, UserDatabase userDatabase)
	{
		UserDomain userDomain = userDatabase.ToDomain();

		userDomain.Username = user.Username ?? userDatabase.Username;
		userDomain.DisplayName = user.DisplayName ?? userDatabase.DisplayName;
		userDomain.Bio = user.Bio ?? userDatabase.Bio;
		userDomain.Image = user.Image ?? userDatabase.Image;

		return userDomain;
	}

	public static UserDatabase ToDatabase(this UserDomain user)
	{
		return new UserDatabase()
		{
			Id = user.Id,
			Username = user.Username,
			DisplayName = user.DisplayName,
			CreatedAt = user.CreatedAt,
			UpdatedAt = user.UpdatedAt,
			Image = user.Image,
			Bio = user.Bio,
			LastActive = user.LastActive,
			Bookmarks = user.Bookmarks.Select(b => b.ToDatabase()).ToList(),
			Reminders = user.Reminders.Select(r => r.ToDatabase()).ToList(),
			UserSettings = user.UserSettings.Select(us => us.ToDatabase()).ToList()
		};
	}
}