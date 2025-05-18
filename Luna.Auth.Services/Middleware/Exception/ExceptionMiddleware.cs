using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Luna.Auth.Services.Middleware.Exception;

public class ExceptionMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionMiddleware> _logger;
	private readonly IHostEnvironment _environment;

	public ExceptionMiddleware(
		RequestDelegate next,
		ILogger<ExceptionMiddleware> logger,
		IHostEnvironment environment)
	{
		_next = next;
		_logger = logger;
		_environment = environment;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (System.Exception ex)
		{
			_logger.LogError(ex, "Exception: {Message}", ex.Message);
			await HandleExceptionAsync(context, ex);
		}
	}

	private async Task HandleExceptionAsync(HttpContext context, System.Exception exception)
	{
		context.Response.ContentType = "application/json";

		ErrorResponse response = _environment.IsDevelopment()
			? new ErrorResponse(exception.Message, exception.StackTrace)
			: new ErrorResponse("Internal server error");

		HttpStatusCode statusCode = exception switch
		{
			ArgumentException => HttpStatusCode.BadRequest,
			KeyNotFoundException => HttpStatusCode.NotFound,
			UnauthorizedAccessException => HttpStatusCode.Unauthorized,
			_ => HttpStatusCode.InternalServerError
		};

		context.Response.StatusCode = (int) statusCode;

		JsonSerializerOptions options = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
		await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
	}
}