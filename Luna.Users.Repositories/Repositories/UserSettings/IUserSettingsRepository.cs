using Luna.Users.Models.Database.Models;

namespace Luna.Users.Repositories.Repositories.UserSettings;

public interface IUserSettingsRepository
{
	Task<UserSettingsDatabase?> GetByUserIdAsync(Guid userId);
	Task<bool> CreateOrUpdateAsync(Guid userId, UserSettingsDatabase settings);
	Task<bool> DeleteByUserIdAsync(Guid userId);
}