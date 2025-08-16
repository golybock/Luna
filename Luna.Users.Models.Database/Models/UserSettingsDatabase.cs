namespace Luna.Users.Models.Database.Models;

public class UserSettingsDatabase
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string? Settings { get; set; }
	public string? Timezone { get; set; }
	public string? Language { get; set; }
}