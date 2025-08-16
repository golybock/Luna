namespace Luna.Tools.SharedModels.Models.Email;

public class AuthCodeEmail
{
	public string RecipientEmail { get; set; }
	public string RecipientName { get; set; }
	public string AuthCode { get; set; }
	public string AppName { get; set; }
	public int ExpirationMinutes { get; set; }
}