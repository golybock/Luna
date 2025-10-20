using System.Text.Json;

namespace Luna.Gateway.API.Middleware;

public class TokenValidationMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly ILogger<TokenValidationMiddleware> _logger;

	public TokenValidationMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory, ILogger<TokenValidationMiddleware> logger)
	{
		_next = next;
		_httpClientFactory = httpClientFactory;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		string? path = context.Request.Path.Value?.ToLower();

		// Пропускаем запросы к Auth.API
		if (path != null && path.Contains("/api/v1/auth"))
		{
			await _next(context);
			return;
		}

		if (!context.Request.Cookies.TryGetValue("access_token", out string? accessToken))
		{
			await WriteUnauthorizedResponse(context, "No access token");
			return;
		}

		if (!context.Request.Cookies.TryGetValue("refresh_token", out string? refreshToken))
		{
			await WriteUnauthorizedResponse(context, "No refresh token");
			return;
		}

		try
		{
			HttpClient client = _httpClientFactory.CreateClient("AuthValidation");

			string cookieString = $"access_token={accessToken}; refresh_token={refreshToken}";
			client.DefaultRequestHeaders.Add("Cookie", cookieString);

			// todo сделать передаваемым как параметр
			HttpResponseMessage response = await client.GetAsync("http://luna.auth.api:8080/api/v1/auth/validate");

			if (!response.IsSuccessStatusCode)
			{
				_logger.LogWarning("Token validation failed with status code: {StatusCode}", response.StatusCode);
				context.Response.StatusCode = (int) response.StatusCode;

				// Удаляем токены если они не валидны
				context.Response.Cookies.Delete("access_token");
				context.Response.Cookies.Delete("refresh_token");

				await context.Response.WriteAsync("Unauthorized: Invalid token");
				return;
			}

			string content = await response.Content.ReadAsStringAsync();

			if (string.IsNullOrEmpty(content))
			{
				_logger.LogWarning("Empty response from auth validation service");
				await WriteUnauthorizedResponse(context, "Invalid response from auth service");
				return;
			}

			Dictionary<string, object>? userData = JsonSerializer.Deserialize<Dictionary<string, object>>(content);

			if (userData != null)
			{
				foreach (var (key, value) in userData)
				{
					context.Request.Headers.Append($"X-User-{key}", value.ToString());
				}
			}

			if (response.Headers.Contains("Set-Cookie"))
			{
				IEnumerable<string> cookies = response.Headers.GetValues("Set-Cookie");
				foreach (string cookie in cookies)
				{
					context.Response.Headers.Append("Set-Cookie", cookie);
				}
			}

			await _next(context);
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, "Network error during token validation");
			context.Response.StatusCode = 503;
			await context.Response.WriteAsync("Service temporarily unavailable");
		}
		catch (JsonException ex)
		{
			_logger.LogError(ex, "Failed to parse auth service response");
			context.Response.StatusCode = 500;
			await context.Response.WriteAsync("Internal server error");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error during token validation");
			context.Response.StatusCode = 500;
			await context.Response.WriteAsync("Internal server error");
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