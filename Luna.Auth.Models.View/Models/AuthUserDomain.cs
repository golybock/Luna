namespace Luna.Auth.Models.View.Models;

public class AuthUserDomain
{
	public Guid Id { get; set; }

	public string Email { get; set; } = null!;

	public DateTime CreatedAt { get; set; }

	public Boolean EmailConfirmed { get; set; }
}