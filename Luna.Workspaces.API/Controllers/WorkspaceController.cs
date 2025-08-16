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
	public async Task<ActionResult<WorkspaceView?>> GetWorkspace(Guid workspaceId)
	{
		WorkspaceView? workspace = await _workspaceService.GetWorkspaceAsync(UserId, workspaceId);

		return workspace != null ? Ok(workspace) : NotFound();
	}

	[HttpGet("[action]")]
	public async Task<ActionResult<WorkspaceDetailedView?>> GetWorkspaceDetailed(Guid workspaceId)
	{
		WorkspaceDetailedView? workspace = await _workspaceService.GetWorkspaceDetailedViewAsync(UserId, workspaceId);

		return workspace != null ? Ok(workspace) : NotFound();
	}


	[HttpGet("[action]")]
	public async Task<ActionResult<IEnumerable<WorkspaceView>>> GetAvailableWorkspaces()
	{
		IEnumerable<WorkspaceView> workspaces = await _workspaceService.GetUserAvailableWorkspacesAsync(UserId);

		return Ok(workspaces);
	}

	[HttpGet("[action]")]
	public async Task<ActionResult<IEnumerable<WorkspaceDetailedView>>> GetAvailableWorkspacesDetailed()
	{
		IEnumerable<WorkspaceDetailedView> workspaces = await _workspaceService.GetUserAvailableWorkspacesDetailedViewAsync(UserId);

		return Ok(workspaces);
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<Guid>> CreateWorkspace(WorkspaceBlank workspaceBlank)
	{
		await _workspaceService.CreateWorkspaceAsync(UserId, workspaceBlank);
		return Ok();
	}

	[HttpPut("[action]")]
	public async Task<ActionResult> UpdateWorkspace(Guid workspaceId, WorkspaceBlank workspaceBlank)
	{
		await _workspaceService.UpdateWorkspaceAsync(UserId, workspaceId, workspaceBlank);
		return Ok();
	}

	[HttpDelete("[action]")]
	public async Task<ActionResult> DeleteWorkspace(Guid workspaceId)
	{
		await _workspaceService.DeleteWorkspaceAsync(UserId, workspaceId);
		return Ok();
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<Guid>> InviteUserToWorkspace(WorkspaceUserBlank workspaceUserBlank)
	{
		Guid id = await _inviteService.CreateInviteAsync(UserId, workspaceUserBlank);
		return Ok(id);
	}

	[HttpGet("[action]")]
	public async Task<ActionResult<WorkspaceUserBlank>> GetWorkspaceInvite(Guid inviteId)
	{
		WorkspaceUserBlank? result = await _inviteService.GetInviteByidAsync(UserId, inviteId);
		return result != null ? Ok(result) : NotFound();
	}

	[HttpPut("[action]")]
	public async Task<ActionResult> UpdateWorkspaceUser(Guid workspaceUserId, WorkspaceUserBlank workspaceUserBlank)
	{
		await _workspaceService.UpdateWorkspaceUserAsync(UserId, workspaceUserId, workspaceUserBlank);
		return Ok();
	}

	[HttpDelete("[action]")]
	public async Task<ActionResult> DeleteWorkspaceUser(Guid workspaceUserId)
	{
		await _workspaceService.DeleteWorkspaceUserAsync(UserId, workspaceUserId);
		return Ok();
	}
}