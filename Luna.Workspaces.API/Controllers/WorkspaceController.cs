using Luna.Tools.SharedModels.Models;
using Luna.Tools.Web;
using Luna.Workspaces.Models.Blank.Models;
using Luna.Workspaces.Models.View.Models;
using Luna.Workspaces.Services.Services.InviteService;
using Luna.Workspaces.Services.Services.WorkspaceService;
using Microsoft.AspNetCore.Mvc;

namespace Luna.Workspaces.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class WorkspaceController : ServiceControllerBase
{
	private readonly IWorkspaceService _workspaceService;
	private readonly IInviteService _inviteService;

	public WorkspaceController(IWorkspaceService workspaceService, IInviteService inviteService)
	{
		_workspaceService = workspaceService;
		_inviteService = inviteService;
	}

	[HttpGet("[action]")]
	public async Task<ActionResult<WorkspaceView>> GetWorkspace(Guid workspaceId)
	{
		WorkspaceView? workspace = await _workspaceService.GetWorkspaceAsync(workspaceId, UserId);

		return workspace != null ? Ok(workspace) : NotFound();
	}

	[HttpGet("[action]")]
	public async Task<ActionResult<WorkspaceDetailedView>> GetWorkspaceDetailed(Guid workspaceId)
	{
		WorkspaceDetailedView? workspace = await _workspaceService.GetWorkspaceDetailedViewAsync(workspaceId, UserId);

		return workspace != null ? Ok(workspace) : NotFound();
	}


	[HttpGet("[action]")]
	public async Task<ActionResult<IEnumerable<WorkspaceView>>> GetAvailableWorkspaces()
	{
		IEnumerable<WorkspaceView> workspaces = await _workspaceService.GetAvailableWorkspacesAsync(UserId);

		return Ok(workspaces);
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<Guid>> CreateWorkspace(WorkspaceBlank workspaceBlank)
	{
		await _workspaceService.CreateWorkspaceAsync(workspaceBlank, UserId);
		return Ok();
	}

	[HttpPut("[action]")]
	public async Task<ActionResult> UpdateWorkspace(Guid workspaceId, WorkspaceBlank workspaceBlank)
	{
		await _workspaceService.UpdateWorkspaceAsync(workspaceId, UserId, workspaceBlank);
		return Ok();
	}

	[HttpDelete("[action]")]
	public async Task<ActionResult> DeleteWorkspace(Guid workspaceId)
	{
		await _workspaceService.DeleteWorkspaceAsync(workspaceId, UserId);
		return Ok();
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<Guid>> InviteUserToWorkspace(WorkspaceUserBlank workspaceUserBlank)
	{
		string id = await _inviteService.CreateInviteAsync(workspaceUserBlank, UserId);
		return Ok(id);
	}

	[HttpPost("[action]")]
	public async Task<ActionResult> AcceptInvite(Guid inviteId)
	{
		await _workspaceService.AcceptInviteAsync(inviteId, UserId);
		return Ok();
	}

	[HttpGet("[action]")]
	public async Task<ActionResult<WorkspaceView>> GetWorkspaceByInvite(Guid inviteId)
	{
		WorkspaceView? workspace = await _workspaceService.GetWorkspaceByInviteAsync(inviteId, UserId);
		return workspace != null ? Ok(workspace) : NotFound();
	}

	[HttpPut("[action]")]
	public async Task<ActionResult> UpdateWorkspaceUser(Guid workspaceUserId, WorkspaceUserBlank workspaceUserBlank)
	{
		await _workspaceService.UpdateWorkspaceUserAsync(workspaceUserId, UserId, workspaceUserBlank);
		return Ok();
	}

	[HttpDelete("[action]")]
	public async Task<ActionResult> DeleteWorkspaceUser(Guid workspaceUserId)
	{
		await _workspaceService.DeleteWorkspaceUserAsync(workspaceUserId, UserId);
		return Ok();
	}

	[HttpGet("[action]")]
	public ActionResult<string[]> GetAvailableWorkspacePermissions()
	{
		return Ok(WorkspacePermissions.AllPermissions);
	}
}