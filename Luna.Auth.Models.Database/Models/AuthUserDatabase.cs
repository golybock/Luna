namespace Luna.Auth.Models.Database.Models;

public class AuthUserDatabase
{
	public Guid Id { get; set; }

	public string Email { get; set; } = null!;

	public Int32 Status { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }

	public Boolean EmailConfirmed { get; set; }

	public String? VerificationToken { get; set; }

	public String? ResetPasswordToken { get; set; }

	public DateTime? ResetTokenExpires { get; set; }
}