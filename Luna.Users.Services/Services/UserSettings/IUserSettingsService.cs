using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.View.Models;

namespace Luna.Users.Services.Services.UserSettings;

public interface IUserSettingsService
{
	Task<UserSettingsView?> GetByUserIdAsync(Guid userId);
	Task<bool> CreateOrUpdateAsync(Guid userId, UserSettingsBlank settings);
	Task<bool> ResetToDefaults(Guid userId);
	Task<bool> DeleteByUserIdAsync(Guid userId);
}