using Luna.Auth.Models.View.Models;
using Luna.Auth.Services.Services.SessionManagementService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = Luna.Tools.Web.ControllerBase;

namespace Luna.Auth.API.Controllers;

[ApiController, Authorize]
[Route("api/v1/[controller]")]
public class SessionController : ControllerBase
{
	private readonly ISessionManagementService _sessionManagementService;

	public SessionController(ISessionManagementService sessionManagementService)
	{
		_sessionManagementService = sessionManagementService;
	}

	[HttpGet("[action]")]
	public async Task<ActionResult<IEnumerable<SessionView>>> GetSessions()
	{
		IEnumerable<SessionView> sessions = await _sessionManagementService.GetUserSessionsAsync(UserId!.Value);

		if (!sessions.Any())
		{
			return NotFound();
		}

		return Ok(sessions);
	}

	[HttpPost("[action]")]
	public async Task<ActionResult> CloseSession(Guid sessionId)
	{
		await _sessionManagementService.CloseSessionAsync(UserId!.Value, sessionId);
		return Ok();
	}

	[HttpPost("[action]")]
	public async Task<ActionResult> CloseAllSessions(bool ignoreCurrentSession = true)
	{
		await _sessionManagementService
			.CloseAllUserSessionsAsync(UserId!.Value, ignoreCurrentSession ? SessionId!.Value : null);

		return Ok();
	}
}