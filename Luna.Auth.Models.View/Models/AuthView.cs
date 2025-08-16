namespace Luna.Auth.Models.View.Models;

public class AuthView
{
	public Guid UserId { get; set; }
	public String Email { get; set; } = string.Empty;
	public Guid SessionId { get; set; }
}