using Luna.Tools.Web;
using Microsoft.AspNetCore.Authentication;

namespace Luna.Auth.Services.Middleware;

public static class AuthenticationBuilderExtensions
{
	public static AuthenticationBuilder AddTokenAuth(
		this AuthenticationBuilder builder,
		string authenticationScheme,
		Action<JwtAuthOptions>? configureOptions = null)
	{
		return builder.AddScheme<JwtAuthOptions, TokenAuthenticationHandler>(
			authenticationScheme,
			configureOptions);
	}
}