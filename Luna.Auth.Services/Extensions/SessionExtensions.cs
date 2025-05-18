using Luna.Auth.Models.Database.Models;
using Luna.Auth.Models.Domain.Models;
using Luna.Auth.Models.View.Models;

namespace Luna.Auth.Services.Extensions;

public static class SessionExtensions
{
	public static SessionView ToView(this SessionDatabase session)
	{
		return new SessionView
		{
			Id = session.Id,
			Device = session.Device,
			CreatedAt = session.CreatedAt,
			ExpiresAt = session.ExpiresAt,
			UserAgent = session.UserAgent,
			IpAddress = session.IpAddress,
			RevokedAt = session.RevokedAt
		};
	}

	// Если нужен промежуточный перевод в доменную модель
	public static SessionDomain ToDomain(this SessionDatabase session)
	{
		return new SessionDomain
		{
			Id = session.Id,
			UserId = session.UserId,
			Token = session.Token,
			RefreshToken = session.RefreshToken,
			Device = session.Device,
			CreatedAt = session.CreatedAt,
			ExpiresAt = session.ExpiresAt,
			UserAgent = session.UserAgent,
			IpAddress = session.IpAddress,
			RevokedAt = session.RevokedAt
		};
	}

	public static SessionView ToView(this SessionDomain domain)
	{
		return new SessionView
		{
			Id = domain.Id,
			Device = domain.Device,
			CreatedAt = domain.CreatedAt,
			ExpiresAt = domain.ExpiresAt,
			UserAgent = domain.UserAgent,
			IpAddress = domain.IpAddress,
			RevokedAt = domain.RevokedAt
		};
	}
}
