using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Luna.Auth.Models.Database.Models;
using Luna.Auth.Repositories.Repositories.SessionRepository;
using Luna.Auth.Services.Services.TokensService;
using Luna.Tools.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Luna.Auth.Services.Middleware;

public class TokenAuthenticationHandler : AuthenticationHandler<JwtAuthOptions>
{
	private readonly ITokensService _tokensService;
	private readonly ISessionRepository _sessionRepository;
	private readonly JwtAuthOptions _jwtOptions;

	public TokenAuthenticationHandler(
		IOptionsMonitor<JwtAuthOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder,
		ISessionRepository sessionRepository,
		ITokensService tokensService
	) : base(options, logger, encoder)
	{
		_jwtOptions = options.CurrentValue;
		_sessionRepository = sessionRepository;
		_tokensService = tokensService;
	}

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		if (!Request.Cookies.TryGetValue("access_token", out string? accessToken) ||
		    !Request.Cookies.TryGetValue("refresh_token", out string? refreshToken))
		{
			return AuthenticateResult.NoResult();
		}

		try
		{
			// Проверяем валидность access токена с проверкой срока действия
			JwtSecurityToken jwtToken = _tokensService.ValidateJwt(accessToken, true);

			// Создаем клеймы на основе JWT токена
			List<Claim> claims = jwtToken.Claims.ToList();
			ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
			ClaimsPrincipal principal = new ClaimsPrincipal(identity);
			AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);

			return AuthenticateResult.Success(ticket);
		}
		catch (InvalidOperationException ex) when (ex.InnerException is SecurityTokenExpiredException)
		{
			Logger.LogDebug("Access token expired, trying refresh token flow");

			try
			{
				// Валидируем подпись/issuer/audience без срока действия
				JwtSecurityToken expiredJwtToken = _tokensService.ValidateJwt(accessToken, false);

				string userId = GetClaimValue(expiredJwtToken, ClaimTypes.NameIdentifier);
				string sessionId = GetClaimValue(expiredJwtToken, "sessionIdentifier");
				string email = GetClaimValue(expiredJwtToken, ClaimTypes.Email);

				if (!Guid.TryParse(userId, out Guid userGuid) ||
				    !Guid.TryParse(sessionId, out Guid sessionGuid) ||
				    string.IsNullOrEmpty(email))
				{
					ClearTokenCookies();
					return AuthenticateResult.Fail("Недействительные данные в токене");
				}

				SessionDatabase? session = await _sessionRepository.GetSessionAsync(
					userGuid,
					sessionGuid
				);

				if (session == null || session.RefreshToken != refreshToken)
				{
					ClearTokenCookies();
					return AuthenticateResult.Fail("Refresh токен недействителен или просрочен");
				}

				List<Claim> newClaims =
				[
					new Claim(ClaimTypes.NameIdentifier, userGuid.ToString()),
					new Claim(ClaimTypes.Email, email),
					new Claim("sessionIdentifier", sessionGuid.ToString())
				];

				string newAccessToken = _tokensService.GenerateAccessToken(newClaims);
				string newRefreshToken = _tokensService.GenerateRefreshToken();

				session.Token = newAccessToken;
				session.RefreshToken = newRefreshToken;

				await _sessionRepository.SetSessionAsync(
					userGuid,
					sessionGuid,
					session,
					TimeSpan.FromDays(_jwtOptions.RefreshValidInDays)
				);

				// Устанавливаем новые токены в куки
				SetTokenCookies(newAccessToken, newRefreshToken);

				// Создаем новые клеймы и тикет аутентификации
				ClaimsIdentity identity = new ClaimsIdentity(newClaims, Scheme.Name);
				ClaimsPrincipal principal = new ClaimsPrincipal(identity);
				AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);

				return AuthenticateResult.Success(ticket);
			}
			catch (System.Exception refreshEx)
			{
				Logger.LogError(refreshEx, "Ошибка при обновлении токенов");
				ClearTokenCookies();
				return AuthenticateResult.Fail("Ошибка аутентификации: " + refreshEx.Message);
			}
		}
		catch (Exception ex)
		{
			Logger.LogWarning(ex, "Invalid access token");
			ClearTokenCookies();
			return AuthenticateResult.Fail("Invalid access token");
		}
	}

	private string GetClaimValue(JwtSecurityToken token, string claimType)
	{
		return token.Claims.FirstOrDefault(c => c.Type == claimType)?.Value ?? string.Empty;
	}

	private void SetTokenCookies(string accessToken, string refreshToken)
	{
		CookieOptions accessTokenOptions = new CookieOptions
		{
			HttpOnly = true,
			Secure = true, // для HTTPS
			SameSite = SameSiteMode.Strict,
			Expires = DateTime.UtcNow.AddDays(_jwtOptions.ValidInDays)
		};

		CookieOptions refreshTokenOptions = new CookieOptions
		{
			HttpOnly = true,
			Secure = true, // для HTTPS
			SameSite = SameSiteMode.Strict,
			Expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshValidInDays)
		};

		Response.Cookies.Append("access_token", accessToken, accessTokenOptions);
		Response.Cookies.Append("refresh_token", refreshToken, refreshTokenOptions);
	}

	private void ClearTokenCookies()
	{
		Response.Cookies.Delete("access_token");
		Response.Cookies.Delete("refresh_token");
	}
}