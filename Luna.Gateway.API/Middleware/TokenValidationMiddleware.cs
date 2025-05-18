using System.Text.Json;

namespace Luna.Gateway.API.Middleware;

public class TokenValidationMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IHttpClientFactory _httpClientFactory;

	public TokenValidationMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory)
	{
		_next = next;
		_httpClientFactory = httpClientFactory;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		string? path = context.Request.Path.Value?.ToLower();

		// Пропускаем запросы к Auth.API
		if (path != null && path.Contains("/api/v1/auth/"))
		{
			await _next(context);
			return;
		}

		if (!context.Request.Cookies.TryGetValue("access_token", out string? accessToken))
		{
			context.Response.StatusCode = 401;
			await context.Response.WriteAsync("Unauthorized: No access token");
			return;
		}

		if (!context.Request.Cookies.TryGetValue("refresh_token", out string? refreshToken))
		{
			context.Response.StatusCode = 401;
			await context.Response.WriteAsync("Unauthorized: No refresh token");
			return;
		}

		try
		{
			HttpClient client = _httpClientFactory.CreateClient("AuthValidation");
			client.DefaultRequestHeaders.Add("Cookie", $"access_token={accessToken}");
			client.DefaultRequestHeaders.Add("Cookie", $"refresh_token={refreshToken}");

			HttpResponseMessage response = await client.PostAsync("http://luna.auth.api:8080/validate", null);


			if (!response.IsSuccessStatusCode)
			{
				context.Response.StatusCode = (int) response.StatusCode;
				await context.Response.WriteAsync("Unauthorized: Invalid token");
				return;
			}

			string content = await response.Content.ReadAsStringAsync();
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
		catch (Exception ex)
		{
			context.Response.StatusCode = 500;
			await context.Response.WriteAsync($"Error validating token: {ex.Message}");
		}
	}
}

public static class TokenValidationMiddlewareExtensions
{
	public static IApplicationBuilder UseTokenValidation(this IApplicationBuilder builder)
	{
		return builder.UseMiddleware<TokenValidationMiddleware>();
	}
}