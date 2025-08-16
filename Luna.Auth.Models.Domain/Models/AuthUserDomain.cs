using Luna.Auth.Models.Enum.Models;

namespace Luna.Auth.Models.Domain.Models;

public class AuthUserDomain
{
	public Guid Id { get; set; }

	public string Email { get; set; } = null!;

	public AuthUserStatus Status { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }

	public Boolean EmailConfirmed { get; set; }

	public String? VerificationToken { get; set; }

	public String? ResetPasswordToken { get; set; }

	public DateTime? ResetTokenExpires { get; set; }
}