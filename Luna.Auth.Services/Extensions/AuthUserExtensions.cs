using Luna.Auth.Models.Database.Models;
using Luna.Auth.Models.Domain.Models;
using Luna.Auth.Models.Enum.Models;

namespace Luna.Auth.Services.Extensions;

public static class AuthUserExtensions
{
	public static AuthUserDomain ToDomain(this AuthUserDatabase authUserDatabase)
	{
		return new AuthUserDomain()
		{
			Id = authUserDatabase.Id,
			Email = authUserDatabase.Email,
			CreatedAt = authUserDatabase.CreatedAt,
			EmailConfirmed = authUserDatabase.EmailConfirmed,
			PasswordHash = authUserDatabase.PasswordHash,
			ResetPasswordToken = authUserDatabase.ResetPasswordToken,
			ResetTokenExpires = authUserDatabase.ResetTokenExpires,
			Status = (AuthUserStatus) authUserDatabase.Status,
			UpdatedAt = authUserDatabase.UpdatedAt,
			VerificationToken =	authUserDatabase.VerificationToken
		};
	}

	public static AuthUserDatabase ToDatabase(this AuthUserDomain authUserDomain)
	{
		return new AuthUserDatabase()
		{
			Id = authUserDomain.Id,
			Email = authUserDomain.Email,
			CreatedAt = authUserDomain.CreatedAt,
			EmailConfirmed = authUserDomain.EmailConfirmed,
			PasswordHash = authUserDomain.PasswordHash,
			ResetPasswordToken = authUserDomain.ResetPasswordToken,
			ResetTokenExpires = authUserDomain.ResetTokenExpires,
			Status = (int) authUserDomain.Status,
			UpdatedAt = authUserDomain.UpdatedAt,
			VerificationToken =	authUserDomain.VerificationToken
		};
	}
}