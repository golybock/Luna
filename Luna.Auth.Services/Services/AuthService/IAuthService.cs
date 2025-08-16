using Luna.Auth.Models.Blank.Models;
using Luna.Auth.Models.Domain.Models;

namespace Luna.Auth.Services.Services.AuthService;

public interface IAuthService
{
	Task RequestVerificationCodeAsync(SignInBlank signInBlank);

	Task<AuthDomain> SignInAsync(SignInCodeBlank signInCodeBlank);

	Task<AuthDomain> LoginOauth2(OAuth2Blank oauth2Blank);

	Task LogoutAsync(Guid userId, Guid sessionId);
}