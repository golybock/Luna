using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace Luna.Gateway.API.Middleware;

public class TokenValidationMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IConfiguration _configuration;
	private readonly ILogger<TokenValidationMiddleware> _logger;
	private readonly StackExchange.Redis.IDatabase _redisDatabase;

	public TokenValidationMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<TokenValidationMiddleware> logger, StackExchange.Redis.IConnectionMultiplexer redis)
	{
		_next = next;
		_configuration = configuration;
		_logger = logger;
		_redisDatabase = redis.GetDatabase();
	}

	public async Task InvokeAsync(HttpContext context)
	{
		string? path = context.Request.Path.Value?.ToLower();

		// Быстрый ответ для теста производительности шлюза (raw benchmark)
		if (path == "/" || path == "/ping")
		{
			context.Response.StatusCode = 200;
			context.Response.ContentType = "text/plain";
			await context.Response.WriteAsync("available");
			return;
		}

		// Пропускаем запросы к Auth.API
		if (path != null && path.Contains("/api/v1/auth"))
		{
			await _next(context);
			return;
		}

		if (!context.Request.Cookies.TryGetValue("access_token", out string? accessToken) || string.IsNullOrEmpty(accessToken))
		{
			await WriteUnauthorizedResponse(context, "No access token");
			return;
		}

		if (!context.Request.Cookies.TryGetValue("refresh_token", out string? refreshToken) || string.IsNullOrEmpty(refreshToken))
		{
			await WriteUnauthorizedResponse(context, "No refresh token");
			return;
		}

		try
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			// Используем тот же секретный ключ, что и в Auth.API
			var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"] ?? throw new Exception("JWT key not set"));

			tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = true,
				ValidIssuer = _configuration["JWT:Issuer"] ?? "http://localhost:7000",
				ValidateAudience = true,
				ValidAudience = _configuration["JWT:Audience"] ?? "user",
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			}, out SecurityToken validatedToken);

			var jwtToken = (JwtSecurityToken)validatedToken;

			// Извлекаем claims
			var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier || x.Type == "sub" || x.Type == "nameid")?.Value;
			var email = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email || x.Type == "email" || x.Type == "emailaddress")?.Value;
			var sessionId = jwtToken.Claims.FirstOrDefault(x => x.Type == "sessionIdentifier")?.Value;

			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sessionId))
			{
				_logger.LogWarning("Required claims missing in JWT token");
				await WriteUnauthorizedResponse(context, "Invalid claims in token");
				return;
			}

			// Проверка активности сессии в Redis (blacklist check)
			string sessionKey = $"{userId}:{sessionId}";
			bool isSessionActive = await _redisDatabase.KeyExistsAsync(sessionKey);
			if (!isSessionActive)
			{
				_logger.LogWarning("Session is inactive or blacklisted in Redis: {SessionKey}", sessionKey);
				await WriteUnauthorizedResponse(context, "Session is inactive or blacklisted");
				return;
			}

			// Добавляем заголовки для нижележащих сервисов
			context.Request.Headers.Append("X-User-UserId", userId);
			context.Request.Headers.Append("X-User-Email", email);
			context.Request.Headers.Append("X-User-SessionId", sessionId);

			if (path == "/test-auth")
			{
				context.Response.StatusCode = 200;
				context.Response.ContentType = "text/plain";
				await context.Response.WriteAsync("authorized");
				return;
			}

			await _next(context);
		}
		catch (SecurityTokenExpiredException ex)
		{
			_logger.LogWarning("Token validation failed: expired");
			context.Response.StatusCode = 401;
			context.Response.Cookies.Delete("access_token");
			context.Response.Cookies.Delete("refresh_token");
			await context.Response.WriteAsync("Unauthorized: Token expired");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Local token validation failed");
			context.Response.StatusCode = 401;
			context.Response.Cookies.Delete("access_token");
			context.Response.Cookies.Delete("refresh_token");
			await context.Response.WriteAsync("Unauthorized: Invalid token");
		}
	}

	private static async Task WriteUnauthorizedResponse(HttpContext context, string message)
	{
		context.Response.StatusCode = 401;
		await context.Response.WriteAsync($"Unauthorized: {message}");
	}
}

public static class TokenValidationMiddlewareExtensions
{
	public static IApplicationBuilder UseTokenValidation(this IApplicationBuilder builder)
	{
		return builder.UseMiddleware<TokenValidationMiddleware>();
	}
}