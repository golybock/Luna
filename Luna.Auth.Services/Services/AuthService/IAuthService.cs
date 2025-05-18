using Luna.Auth.Models.Blank.Models;
using Luna.Auth.Models.Domain.Models;


namespace Luna.Auth.Services.Services.AuthService;

public interface IAuthService
{
	Task<AuthDomain> LoginAsync(SignInBlank signInBlank);

	Task<AuthDomain> RegisterAsync(SignUpBlank signUpBlank);

	Task<AuthDomain> LoginOauth2(string email);

	Task LogoutAsync(Guid userId, Guid sessionId);
}