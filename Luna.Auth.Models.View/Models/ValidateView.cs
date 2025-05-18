namespace Luna.Auth.Models.View.Models;

public class ValidateView
{
	public Guid UserId { get; set; }

	public Guid SessionId { get; set; }

	public String Email { get; set; } = string.Empty;

	public Boolean IsValid { get; set; }

	public String? ErrorMessage { get; set; }

	public String? NewToken { get; set; }

	public String? NewRefreshToken { get; set; }
}