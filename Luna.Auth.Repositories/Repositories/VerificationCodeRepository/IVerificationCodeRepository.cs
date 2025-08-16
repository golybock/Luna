using Luna.Auth.Models.Database.Models;

namespace Luna.Auth.Repositories.Repositories.VerificationCodeRepository;

public interface IVerificationCodeRepository
{
	Task CreateVerificationCodeAsync(string email, VerificationCodeDatabase verificationCode, TimeSpan expire);

	Task<VerificationCodeDatabase?> GetVerificationCodeAsync(string email);

	Task DeleteVerificationCodeAsync(string email);
}