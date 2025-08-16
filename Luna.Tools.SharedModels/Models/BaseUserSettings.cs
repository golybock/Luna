namespace Luna.Tools.SharedModels.Models;

public abstract class BaseUserSettings
{
	public string? Theme { get; set; }
	public bool NotificationsEnabled { get; set; }
	public string? DateFormat { get; set; }
	public string? TimeFormat { get; set; }
}