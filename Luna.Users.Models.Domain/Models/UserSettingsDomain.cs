
using Luna.Tools.SharedModels.Models;

namespace Luna.Users.Models.Domain.Models;

public class UserSettingsDomain : BaseUserSettings
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string? Timezone { get; set; }
	public string? Language { get; set; }
}