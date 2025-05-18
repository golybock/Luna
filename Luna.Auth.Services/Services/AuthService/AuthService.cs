using System.Security.Claims;
using Luna.Auth.Models.Blank.Models;
using Luna.Auth.Models.Database.Models;
using Luna.Auth.Models.Domain.Models;
using Luna.Auth.Models.View.Models;
using Luna.Auth.Repositories.Repositories.AuthRepository;
using Luna.Auth.Repositories.Repositories.SessionArchiveRepository;
using Luna.Auth.Repositories.Repositories.SessionRepository;
using Luna.Auth.Services.Extensions;
using Luna.Auth.Services.Services.TokensService;
using Luna.Tools.Crypto;
using Luna.Tools.Web;

namespace Luna.Auth.Services.Services.AuthService;

public class AuthService : IAuthService
{
	private readonly IAuthRepository _authRepository;
	private readonly ISessionRepository _sessionRepository;
	private readonly ISessionArchiveRepository _sessionArchiveRepository;
	private readonly ITokensService _tokensService;
	private readonly JwtOptions _jwtOptions;

	public AuthService(IAuthRepository authRepository, ISessionRepository sessionRepository,
		ISessionArchiveRepository sessionArchiveRepository, ITokensService tokensService, JwtOptions jwtOptions)
	{
		_authRepository = authRepository;
		_sessionRepository = sessionRepository;
		_sessionArchiveRepository = sessionArchiveRepository;
		_tokensService = tokensService;
		_jwtOptions = jwtOptions;
	}

	public async Task<AuthDomain> LoginAsync(SignInBlank signInBlank)
	{
		AuthUserDatabase? user = await _authRepository.GetAuthUserAsync(signInBlank.Email);

		if (user == null)
		{
			throw new UnauthorizedAccessException("User not exists");
		}

		AuthUserDomain userDomain = user.ToDomain();

		if (userDomain.PasswordHash == null)
		{
			throw new UnauthorizedAccessException("User password hash not set");
		}

		bool passwordIsValid = Crypto
			.HashSha512(signInBlank.Password)
			.SequenceEqual(userDomain.PasswordHash);

		if (!passwordIsValid)
		{
			throw new UnauthorizedAccessException("Invalid password");
		}

		Guid sessionId = Guid.NewGuid();

		string token = _tokensService.GenerateAccessToken(GetUserClaims(userDomain.Id, sessionId, userDomain.Email));
		string refreshToken = _tokensService.GenerateRefreshToken();

		AuthDomain authView = new AuthDomain()
		{
			Email = userDomain.Email,
			Token = token,
			RefreshToken = refreshToken,
			UserId = userDomain.Id
		};

		await CreateSessionAsync(sessionId, authView);

		return authView;
	}

	public async Task<AuthDomain> RegisterAsync(SignUpBlank signUpBlank)
	{
		AuthUserDatabase? existsUser = await _authRepository.GetAuthUserAsync(signUpBlank.Email);

		if (existsUser != null)
		{
			throw new Exception("User exists");
		}

		byte[] passwordHash = await Crypto.HashSha512Async(signUpBlank.Password);

		AuthUserDomain newUser = new AuthUserDomain()
		{
			Id = Guid.NewGuid(),
			Email = signUpBlank.Email,
			PasswordHash = passwordHash
		};

		Boolean createdUser = await _authRepository.CreateAuthUserAsync(newUser.ToDatabase());

		if (!createdUser)
		{
			throw new Exception("User not saved");
		}

		Guid sessionId = Guid.NewGuid();

		string token = _tokensService.GenerateAccessToken(GetUserClaims(newUser.Id, sessionId, newUser.Email));
		string refreshToken = _tokensService.GenerateRefreshToken();

		AuthDomain authView = new AuthDomain()
		{
			Email = newUser.Email,
			Token = token,
			RefreshToken = refreshToken,
			UserId = newUser.Id
		};

		await CreateSessionAsync(sessionId, authView);

		return authView;
	}

	public async Task<AuthDomain> LoginOauth2(string email)
	{
		AuthUserDatabase? user = await _authRepository.GetAuthUserAsync(email);

		AuthUserDomain newUser;

		if (user == null)
		{
			newUser = new AuthUserDomain()
			{
				Id = Guid.NewGuid(),
				Email = email,
			};

			Boolean createdUser = await _authRepository.CreateAuthUserAsync(newUser.ToDatabase());

			if (!createdUser)
			{
				throw new Exception("User not saved");
			}
		}
		else
		{
			newUser = user.ToDomain();
		}

		Guid sessionId = Guid.NewGuid();

		string token = _tokensService.GenerateAccessToken(GetUserClaims(newUser.Id, sessionId, newUser.Email));
		string refreshToken = _tokensService.GenerateRefreshToken();

		AuthDomain authView = new AuthDomain()
		{
			Email = newUser.Email,
			Token = token,
			RefreshToken = refreshToken,
			UserId = newUser.Id
		};

		await CreateSessionAsync(sessionId, authView);

		return authView;
	}

	public async Task LogoutAsync(Guid userId, Guid sessionId)
	{
		SessionDatabase? session = await _sessionRepository.GetSessionAsync(userId, sessionId);

		if (session == null)
		{
			throw new UnauthorizedAccessException($"Сессия {sessionId} для пользователя {userId} не найдена");
		}

		bool archived = await _sessionArchiveRepository.CreateSessionAsync(session);

		if (!archived)
		{
			throw new UnauthorizedAccessException("Failed to archive session");
		}

		int closedSessions = await _sessionRepository.CloseSessionAsync(userId, sessionId);

		if (closedSessions == 0)
		{
			await _sessionArchiveRepository.DeleteSessionAsync(session.Id);

			throw new Exception("Failed to close session in Redis");
		}
	}

	private List<Claim> GetUserClaims(Guid userId, Guid sessionId, string email)
	{
		return
		[
			new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
			new Claim(ClaimTypes.Email, email),
			new Claim("sessionIdentifier", sessionId.ToString()),
		];
	}

	private async Task CreateSessionAsync(Guid sessionId, AuthDomain authView, DateTime? createdAt = null)
	{
		TimeSpan sessionValidTimeSpan = TimeSpan.FromDays(_jwtOptions.RefreshValidInDays);

		SessionDatabase sessionDatabase = new SessionDatabase()
		{
			UserId = authView.UserId,
			RefreshToken = authView.RefreshToken,
			Token = authView.Token,
			Id = sessionId,
			ExpiresAt = DateTime.UtcNow.Add(sessionValidTimeSpan),
			CreatedAt = createdAt ?? DateTime.UtcNow,
		};

		await _sessionRepository.SetSessionAsync(authView.UserId, sessionId, sessionDatabase, sessionValidTimeSpan);
	}
}