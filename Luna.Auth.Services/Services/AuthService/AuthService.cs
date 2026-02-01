using System.Security.Claims;
using System.Text.Json;
using Luna.Auth.Models.Blank.Models;
using Luna.Auth.Models.Database.Models;
using Luna.Auth.Models.Domain.Models;
using Luna.Auth.Repositories.Repositories.AuthRepository;
using Luna.Auth.Repositories.Repositories.OutboxRepository;
using Luna.Auth.Repositories.Repositories.SessionArchiveRepository;
using Luna.Auth.Repositories.Repositories.SessionRepository;
using Luna.Auth.Repositories.Repositories.VerificationCodeRepository;
using Luna.Auth.Services.Extensions;
using Luna.Auth.Services.Services.TokensService;
using Luna.Tools.SharedModels.Models.Email;
using Luna.Tools.SharedModels.Models.Outbox;
using Luna.Tools.Web;
using Luna.Users.gRPC.Client.Services;
using Luna.Users.Models.Blank.Models;
using Microsoft.Extensions.Configuration;

namespace Luna.Auth.Services.Services.AuthService;

public class AuthService : IAuthService
{
	private readonly TimeSpan _verificationCodeLifetime;

	private readonly IAuthRepository _authRepository;
	private readonly ISessionRepository _sessionRepository;
	private readonly ISessionArchiveRepository _sessionArchiveRepository;
	private readonly IVerificationCodeRepository _verificationCodeRepository;
	private readonly ITokensService _tokensService;
	private readonly IOutboxRepository _outboxRepository;
	private readonly JwtOptions _jwtOptions;
	private readonly IUserServiceClient _userServiceClient;

	public AuthService
	(
		IAuthRepository authRepository,
		ISessionRepository sessionRepository,
		ISessionArchiveRepository sessionArchiveRepository,
		ITokensService tokensService,
		JwtOptions jwtOptions,
		IUserServiceClient userServiceClient,
		IOutboxRepository outboxRepository,
		IVerificationCodeRepository verificationCodeRepository,
		IConfiguration configuration
	)
	{
		_authRepository = authRepository;
		_sessionRepository = sessionRepository;
		_sessionArchiveRepository = sessionArchiveRepository;
		_tokensService = tokensService;
		_jwtOptions = jwtOptions;
		_userServiceClient = userServiceClient;
		_outboxRepository = outboxRepository;
		_verificationCodeRepository = verificationCodeRepository;
		int lifetimeMinutes = configuration.GetValue("Auth:VerificationCodeLifetimeMinutes", 1);
		_verificationCodeLifetime = TimeSpan.FromMinutes(Math.Max(1, lifetimeMinutes));
	}

	public async Task RequestVerificationCodeAsync(SignInBlank signInBlank)
	{
		if (IsVerificationCodeIgnored())
		{
			return;
		}

		VerificationCodeDatabase? verificationCodeDatabase =
			await _verificationCodeRepository.GetVerificationCodeAsync(signInBlank.Email);

		if (verificationCodeDatabase != null)
		{
			throw new Exception("Verification code already sent, try again in a 1 minute.");
		}

		string code = GenerateRandomString(6);

		VerificationCodeDatabase verificationCode = new VerificationCodeDatabase()
		{
			Code = code,
			CreatedAt = DateTime.UtcNow
		};

		AuthCodeEmail authCodeEmail = new AuthCodeEmail()
		{
			AppName = "Luna",
			AuthCode = code,
			RecipientName = signInBlank.Email,
			RecipientEmail = signInBlank.Email,
			ExpirationMinutes = 10
		};

		// todo вынести время жизни
		await _verificationCodeRepository.CreateVerificationCodeAsync(signInBlank.Email, verificationCode,
			_verificationCodeLifetime);

		OutboxMessageDatabase outboxMessage = new OutboxMessageDatabase()
		{
			Id = Guid.NewGuid(),
			Type = OutboxMessageTypes.AuthCodeEmail,
			Payload = JsonSerializer.Serialize(authCodeEmail),
			Status = OutboxMessageStatus.Pending,
			Attempts = 0,
			CreatedAt = DateTime.UtcNow
		};

		bool saved = await _outboxRepository.AddMessageAsync(outboxMessage);
		if (!saved)
		{
			throw new Exception("Failed to save outbox message");
		}
	}

	public async Task<AuthDomain> SignInAsync(SignInCodeBlank signInCodeBlank)
	{
		if (!IsVerificationCodeIgnored())
		{
			VerificationCodeDatabase? code =
				await _verificationCodeRepository.GetVerificationCodeAsync(signInCodeBlank.Email);

			if (code == null || code.Code != signInCodeBlank.Code)
			{
				throw new UnauthorizedAccessException("Invalid code");
			}

			if (DateTime.UtcNow - code.CreatedAt > _verificationCodeLifetime)
			{
				throw new UnauthorizedAccessException("Code lifetime expired");
			}

			await _verificationCodeRepository.DeleteVerificationCodeAsync(signInCodeBlank.Email);
		}

		AuthUserDomain newUser;
		AuthUserDatabase? user = await _authRepository.GetAuthUserAsync(signInCodeBlank.Email);

		if (user == null)
		{
			newUser = new AuthUserDomain()
			{
				Id = Guid.NewGuid(),
				Email = signInCodeBlank.Email,
			};

			Boolean createdUser = await _authRepository.CreateAuthUserAsync(newUser.ToDatabase());

			if (!createdUser)
			{
				throw new Exception("User not saved");
			}

			// Создаем пользователя на сервисе пользователей через gRPC,
			// в случае ошибки откатываем созданного пользователя в сервисе авторизации
			try
			{
				await _userServiceClient.CreateUserAsync(newUser.Id, new UserBlank());
			}
			catch (Exception e)
			{
				await _authRepository.DeleteAuthUserAsync(newUser.Id);
				throw;
			}
		}
		else
		{
			newUser = user.ToDomain();
		}

		AuthDomain authDomain = CreateAuthDomainAsync(newUser.Id, newUser.Email);
		await SaveSessionAsync(authDomain);

		return authDomain;
	}

	public async Task<AuthDomain> LoginOauth2(OAuth2Blank oAuth2Blank)
	{
		AuthUserDomain newUser;
		AuthUserDatabase? user = await _authRepository.GetAuthUserAsync(oAuth2Blank.Email);

		if (user == null)
		{
			newUser = new AuthUserDomain()
			{
				Id = Guid.NewGuid(),
				Email = oAuth2Blank.Email,
			};

			Boolean createdUser = await _authRepository.CreateAuthUserAsync(newUser.ToDatabase());

			if (!createdUser)
			{
				throw new Exception("User not saved");
			}

			// Создаем пользователя на сервисе пользователей через gRPC,
			// в случае ошибки откатываем созданного пользователя в сервисе авторизации
			try
			{
				await _userServiceClient.CreateUserAsync(newUser.Id,
					new UserBlank() {Username = oAuth2Blank.Username, Image = oAuth2Blank.ImageLink});
			}
			catch (Exception e)
			{
				await _authRepository.DeleteAuthUserAsync(newUser.Id);
				throw;
			}
		}
		else
		{
			newUser = user.ToDomain();
		}

		AuthDomain authDomain = CreateAuthDomainAsync(newUser.Id, newUser.Email);
		await SaveSessionAsync(authDomain);

		return authDomain;
	}

	private AuthDomain CreateAuthDomainAsync(Guid userId, string email)
	{
		Guid sessionId = Guid.NewGuid();

		List<Claim> claims = GetUserClaims(userId, sessionId, email);
		string token = _tokensService.GenerateAccessToken(claims);
		string refreshToken = _tokensService.GenerateRefreshToken();

		return new AuthDomain()
		{
			UserId = userId,
			Email = email,
			Token = token,
			RefreshToken = refreshToken,
			SessionId = sessionId,
		};
	}

	private async Task SaveSessionAsync(AuthDomain authDomain)
	{
		await CreateSessionAsync(authDomain);
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

	private async Task CreateSessionAsync(AuthDomain authView, DateTime? createdAt = null)
	{
		TimeSpan sessionValidTimeSpan = TimeSpan.FromDays(_jwtOptions.RefreshValidInDays);

		SessionDatabase sessionDatabase = new SessionDatabase()
		{
			UserId = authView.UserId,
			RefreshToken = authView.RefreshToken,
			Token = authView.Token,
			Id = authView.SessionId,
			ExpiresAt = DateTime.UtcNow.Add(sessionValidTimeSpan),
			CreatedAt = createdAt ?? DateTime.UtcNow,
		};

		await _sessionRepository.SetSessionAsync(authView.UserId, authView.SessionId, sessionDatabase,
			sessionValidTimeSpan);
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

	private static bool IsVerificationCodeIgnored()
	{
		string? value = Environment.GetEnvironmentVariable("IGNORE_VERIFICATION_CODE");
		return bool.TryParse(value, out bool result) && result;
	}
}