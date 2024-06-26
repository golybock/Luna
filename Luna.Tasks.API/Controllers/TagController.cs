﻿using Luna.Models.Tasks.Blank.CardAttributes;
using Luna.Models.Tasks.View.CardAttributes;
using Luna.Tasks.Services.Services.CardAttributes.Tag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = Luna.Tools.Web.ControllerBase;

namespace Luna.Tasks.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
	private readonly ITagService _tagService;

	public TagController(ITagService tagService)
	{
		_tagService = tagService;
	}

	[HttpGet("[action]")]
	public async Task<IEnumerable<TagView>> GetTagsAsync(Guid workspaceId)
	{
		return await _tagService.GetTagsAsync(workspaceId);
	}

	[HttpGet("[action]")]
	public async Task<TagView?> GetTagAsync(Guid id)
	{
		return await _tagService.GetTagAsync(id);
	}

	[HttpPost("[action]")]
	public async Task<IActionResult> CreateTagAsync(TagBlank tag)
	{
		var result = await _tagService.CreateTagAsync(tag, UserId);

		return result ? Ok() : BadRequest();
	}

	[HttpPut("[action]")]
	public async Task<IActionResult> UpdateTagAsync(Guid id, TagBlank tag)
	{
		var result = await _tagService.UpdateTagAsync(id, tag);

		return result ? Ok() : BadRequest();
	}

	[HttpDelete("[action]")]
	public async Task<IActionResult> DeleteTagAsync(Guid id)
	{
		var result = await _tagService.TrashTagAsync(id);

		return result ? Ok() : BadRequest();
	}
}