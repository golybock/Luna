namespace Luna.Tools.SharedModels.Models.Email;

// init from appsettings
public class EmailSettings
{
	public string SmtpHost { get; set; }
	public int SmtpPort { get; set; }
	public string SenderEmail { get; set; }
	public string SenderPassword { get; set; }
	public string SenderName { get; set; }
	public bool EnableSsl { get; set; }
}