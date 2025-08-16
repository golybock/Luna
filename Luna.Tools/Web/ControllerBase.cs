using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Luna.Tools.Web;

public abstract class ControllerBase : Controller
{
	private const string SessionIdentifierType = "sessionIdentifier";

	protected string? Email => User.FindFirst(ClaimTypes.Email)?.Value;

	protected Guid? UserId
	{
		get
		{
			string? id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			return string.IsNullOrEmpty(id) ? null : Guid.Parse(id);
		}
	}

	protected Guid? SessionId
	{
		get
		{
			string? id = User.FindFirst(SessionIdentifierType)?.Value;
			return string.IsNullOrEmpty(id) ? null : Guid.Parse(id);
		}
	}
}