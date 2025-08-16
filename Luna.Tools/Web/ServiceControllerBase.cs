using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Luna.Tools.Web;

public class ServiceControllerBase : Controller
{
	protected Guid UserId
	{
		get
		{
			if (HttpContext.Request.Headers.TryGetValue("X-User-UserId", out StringValues userIdHeader) &&
			    Guid.TryParse(userIdHeader.FirstOrDefault(), out Guid userId))
			{
				return userId;
			}
			throw new UnauthorizedAccessException("UserId not found in request headers");
		}
	}

	protected Guid SessionId
	{
		get
		{
			if (HttpContext.Request.Headers.TryGetValue("X-User-SessionId", out var sessionIdHeader) &&
			    Guid.TryParse(sessionIdHeader.FirstOrDefault(), out var sessionId))
			{
				return sessionId;
			}
			throw new UnauthorizedAccessException("SessionId not found in request headers");
		}
	}

	protected string Email
	{
		get
		{
			if (HttpContext.Request.Headers.TryGetValue("X-User-Email", out StringValues emailHeader))
			{
				string? email = emailHeader.FirstOrDefault();
				if (!string.IsNullOrEmpty(email))
					return email;
			}
			throw new UnauthorizedAccessException("Email not found in request headers");
		}
	}
}