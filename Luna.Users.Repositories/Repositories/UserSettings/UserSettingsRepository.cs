using Luna.Users.Models.Database.Models;
using Luna.Users.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Luna.Users.Repositories.Repositories.UserSettings;

public class UserSettingsRepository : IUserSettingsRepository
{
	private readonly LunaUsersContext _context;

	public UserSettingsRepository(LunaUsersContext context)
	{
		_context = context;
	}

	public async Task<UserSettingsDatabase?> GetByUserIdAsync(Guid userId)
	{
		return await _context.UserSettings
			.AsNoTracking()
			.FirstOrDefaultAsync(s => s.UserId == userId);
	}

	public async Task<bool> CreateOrUpdateAsync(Guid userId, UserSettingsDatabase settings)
	{
		UserSettingsDatabase? existing = await _context.UserSettings
			.FirstOrDefaultAsync(s => s.UserId == userId);

		if (existing != null)
		{
			existing.Settings = settings.Settings;
			existing.Timezone = settings.Timezone;
			existing.Language = settings.Language;
		}
		else
		{
			settings.Id = Guid.NewGuid();
			settings.UserId = userId;
			_context.UserSettings.Add(settings);
		}

		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> DeleteByUserIdAsync(Guid userId)
	{
		int deleted = await _context.UserSettings
			.Where(s => s.UserId == userId)
			.ExecuteDeleteAsync();

		return deleted > 0;
	}
}