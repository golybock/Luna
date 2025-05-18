namespace Luna.Auth.Models.Domain.Models;

public class AuthDomain
{
	public Guid UserId { get; set; }
	public String Email { get; set; } = string.Empty;
	public String Token { get; set; } = string.Empty;
	public String RefreshToken { get; set; } = string.Empty;
}