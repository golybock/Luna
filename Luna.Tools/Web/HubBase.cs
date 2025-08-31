using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace Luna.Tools.Web;

public abstract class HubBase : Hub
{
	protected Guid? UserId
	{
		get
		{
			if (Context.GetHttpContext().Request.Headers.TryGetValue("X-User-UserId", out StringValues userIdHeader) &&
			    Guid.TryParse(userIdHeader.FirstOrDefault(), out Guid userId))
			{
				return userId;
			}
			throw new UnauthorizedAccessException("UserId not found in request headers");
		}
	}

	protected string? Email
	{
		get
		{
			if (Context.GetHttpContext().Request.Headers.TryGetValue("X-User-Email", out StringValues emailHeader))
			{
				string? email = emailHeader.FirstOrDefault();
				if (!string.IsNullOrEmpty(email))
					return email;
			}
			throw new UnauthorizedAccessException("Email not found in request headers");
		}
	}
}