namespace Luna.Auth.Services.Services.AccountManagementService;

// отвечает за проверку почты, сброс пароля и тд
public interface IAccountManagementService
{
	Task RequestEmailVerificationAsync(Guid userId);

	Task VerifyEmailAsync(string verificationToken);

	Task RequestPasswordResetAsync(string email);

	Task ResetPasswordAsync(string resetToken, string newPassword);

	Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
}
