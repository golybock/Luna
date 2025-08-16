using Luna.Tools.SharedModels.Models;

namespace Luna.Users.Models.View.Models;

public class UserSettingsView : BaseUserSettings
{
	public string? Timezone { get; set; }
	public string? Language { get; set; }
}