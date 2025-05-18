using Microsoft.AspNetCore.Builder;

namespace Luna.Auth.Services.Middleware.Exception;

public static class ExceptionMiddlewareExtensions
{
	public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
	{
		return app.UseMiddleware<ExceptionMiddleware>();
	}
}