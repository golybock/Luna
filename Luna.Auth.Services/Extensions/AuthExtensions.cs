using Luna.Auth.Models.Domain.Models;
using Luna.Auth.Models.View.Models;

namespace Luna.Auth.Services.Extensions;

public static class AuthExtensions
{
	public static AuthView ToView(this AuthDomain authDomain)
	{
		return new AuthView()
		{
			Email = authDomain.Email,
			UserId = authDomain.UserId,
		};
	}
}