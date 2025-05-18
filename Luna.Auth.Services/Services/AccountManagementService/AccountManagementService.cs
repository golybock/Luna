using Luna.Auth.Models.Database.Models;
using Luna.Auth.Models.Domain.Models;
using Luna.Auth.Repositories.Repositories.AuthRepository;
using Luna.Auth.Services.Extensions;
using Luna.Tools.Crypto;

namespace Luna.Auth.Services.Services.AccountManagementService;

public class AccountManagementService : IAccountManagementService
{
	private readonly IAuthRepository _authRepository;

	public AccountManagementService(IAuthRepository authRepository)
	{
		_authRepository = authRepository;
	}

	public async Task RequestEmailVerificationAsync(Guid userId)
	{
		AuthUserDatabase? user = await _authRepository.GetAuthUserAsync(userId);

		if (user == null)
		{
			throw new UnauthorizedAccessException("User not exists");
		}

		AuthUserDomain userDomain = user.ToDomain();

		if (userDomain.EmailConfirmed)
		{
			throw new UnauthorizedAccessException("Email already confirmed");
		}

		userDomain.VerificationToken = GenerateRandomString(64);

		await _authRepository.UpdateAuthUserAsync(userId, userDomain.ToDatabase());
	}

	public async Task VerifyEmailAsync(string verificationToken)
	{
		AuthUserDatabase? user = await _authRepository.GetAuthUserByEmailTokenAsync(verificationToken);

		if (user == null)
		{
			throw new UnauthorizedAccessException("Invalid token");
		}

		AuthUserDomain userDomain = user.ToDomain();

		userDomain.VerificationToken = null;
		userDomain.EmailConfirmed = true;

		await _authRepository.UpdateAuthUserAsync(user.Id, userDomain.ToDatabase());
	}

	public async Task RequestPasswordResetAsync(string email)
	{
		AuthUserDatabase? user = await _authRepository.GetAuthUserAsync(email);

		if (user == null)
		{
			throw new UnauthorizedAccessException("User not exists");
		}

		AuthUserDomain userDomain = user.ToDomain();

		userDomain.ResetPasswordToken = GenerateRandomString(64);
		userDomain.ResetTokenExpires = DateTime.UtcNow.AddMinutes(30); //todo вынести глобально

		await _authRepository.UpdateAuthUserAsync(user.Id, userDomain.ToDatabase());
	}

	public async Task ResetPasswordAsync(string resetToken, string newPassword)
	{
		AuthUserDatabase? user = await _authRepository.GetAuthUserByResetTokenAsync(resetToken);

		if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
		{
			throw new UnauthorizedAccessException("Token invalid");
		}

		AuthUserDomain userDomain = user.ToDomain();

		userDomain.PasswordHash = await Crypto.HashSha512Async(newPassword);
		userDomain.ResetTokenExpires = null;
		userDomain.ResetPasswordToken = null;

		await _authRepository.UpdateAuthUserAsync(user.Id, userDomain.ToDatabase());
	}

	public async Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
	{
		AuthUserDatabase? user = await _authRepository.GetAuthUserAsync(userId);

		if (user == null || !(await Crypto.HashSha512Async(oldPassword)).SequenceEqual(user.PasswordHash ?? []))
		{
			throw new UnauthorizedAccessException("Password invalid");
		}

		AuthUserDomain userDomain = user.ToDomain();

		userDomain.PasswordHash = await Crypto.HashSha512Async(newPassword);
		userDomain.ResetTokenExpires = null;
		userDomain.ResetPasswordToken = null;

		await _authRepository.UpdateAuthUserAsync(user.Id, userDomain.ToDatabase());
	}

	private string GenerateRandomString(int length)
	{
		const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		Random random = new Random();
		char[] chars = new char[length];

		for (int i = 0; i < length; i++)
		{
			chars[i] = validChars[random.Next(validChars.Length)];
		}

		return new string(chars);
	}
}