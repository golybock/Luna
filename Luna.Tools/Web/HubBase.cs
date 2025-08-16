using Microsoft.AspNetCore.SignalR;

namespace Luna.Tools.Web;

public abstract class HubBase : Hub
{
	protected Guid? GetUserIdFromCookie()
	{
		return Context.GetHttpContext()?.Request.Cookies.TryGetValue("X-User-UserId", out string? userId) == true
			? Guid.Parse(userId)
			: null;
	}

	protected string? GetUserEmailFromCookie()
	{
		return Context.GetHttpContext()?.Request.Cookies.TryGetValue("X-User-Email", out string? email) == true
			? email
			: null;
	}
}