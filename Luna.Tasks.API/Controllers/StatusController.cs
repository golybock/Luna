﻿using Luna.Models.Tasks.Blank.CardAttributes;
using Luna.Models.Tasks.View.CardAttributes;
using Luna.Tasks.Services.Services.CardAttributes.Status;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = Luna.Tools.Web.ControllerBase;

namespace Luna.Tasks.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
	private readonly IStatusService _statusService;

	public StatusController(IStatusService statusService)
	{
		_statusService = statusService;
	}

	[HttpGet("[action]")]
	public async Task<IEnumerable<StatusView>> GetStatusesAsync(Guid workspaceId)
	{
		return await _statusService.GetStatusesAsync(workspaceId);
	}

	[HttpGet("[action]")]
	public async Task<StatusView?> GetStatusAsync(Guid statusId)
	{
		return await _statusService.GetStatusAsync(statusId);
	}

	[HttpPost("[action]")]
	public async Task<IActionResult> CreateStatusAsync(StatusBlank statusBlank)
	{
		var result = await _statusService.CreateStatusAsync(statusBlank, UserId);

		return result ? Ok() : BadRequest();
	}

	[HttpPut("[action]")]
	public async Task<IActionResult> UpdateStatusAsync(Guid id, StatusBlank statusBlank)
	{
		var result = await _statusService.UpdateStatusAsync(id, statusBlank, UserId);

		return result ? Ok() : BadRequest();
	}

	[HttpDelete("[action]")]
	public async Task<IActionResult> DeleteStatusAsync(Guid id)
	{
		var result = await _statusService.TrashStatusAsync(id, UserId);

		return result ? Ok() : BadRequest();
	}
}