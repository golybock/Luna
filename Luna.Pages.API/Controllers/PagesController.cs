using Luna.Pages.Models.Blank.Models;
using Luna.Pages.Models.View.Additional;
using Luna.Pages.Services.Services.PageService;
using Luna.Tools.SharedModels.Models.API;
using Luna.Tools.Web;
using Microsoft.AspNetCore.Mvc;

namespace Luna.Pages.API.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class PagesController : ServiceControllerBase
{
	private readonly IPageService _pageService;

	public PagesController(IPageService pageService)
	{
		_pageService = pageService;
	}

	#region GET

	[HttpGet("[action]")]
	public async Task<ActionResult<LightPageView>> GetLightPage(Guid pageId)
	{
		GetRequest request = new GetRequest() {Id = pageId, UserId = UserId};

		LightPageView? result = await _pageService.GetPageLightViewAsync(request);

		return result != null ? Ok(result) : NotFound();
	}

	[HttpGet("[action]")]
	public async Task<IEnumerable<LightPageView>> SearchPagesByTitle(Guid workspaceId, string title)
	{
		GetRequest request = new GetRequest() {Id = workspaceId, UserId = UserId};

		return await _pageService.SearchPagesByTitleAsync(request, title);
	}


	[HttpGet("[action]")]
	public async Task<ActionResult<List<LightPageView>>> SearchPages(
		[FromQuery] string query, [FromQuery] Guid workspaceId,
		[FromQuery] int from = 0, [FromQuery] int size = 10)
	{
		SearchGetRequest request = new SearchGetRequest()
		{
			WorkspaceId = workspaceId,
			Query = query,
			From = from,
			Size = size,
			UserId = UserId
		};

		List<LightPageView> results = await _pageService.SearchPagesAsync(request);

		return Ok(results);
	}

	[HttpGet("[action]")]
	public async Task<ActionResult<List<SearchPageBlockView>>> SearchInBlocks(
		[FromQuery] string query, [FromQuery] Guid workspaceId,
		[FromQuery] int from = 0, [FromQuery] int size = 10)
	{
		SearchGetRequest request = new SearchGetRequest()
		{
			WorkspaceId = workspaceId,
			Query = query,
			From = from,
			Size = size,
			UserId = UserId
		};

		List<SearchPageBlockView> results = await _pageService.SearchInBlocksAsync(request);
		return Ok(results);
	}

	[HttpGet("[action]")]
	public async Task<IEnumerable<LightPageView>> GetWorkspacePages(Guid workspaceId)
	{
		GetRequest request = new GetRequest() {Id = workspaceId, UserId = UserId};

		return await _pageService.GetWorkspacePagesAsync(request);
	}

	[HttpGet("[action]")]
	public async Task<IEnumerable<LightPageView>> GetArchivedPages(Guid workspaceId)
	{
		GetRequest request = new GetRequest() {Id = workspaceId, UserId = UserId};

		return await _pageService.GetArchivedPagesAsync(request);
	}

	[HttpGet("[action]")]
	public async Task<IEnumerable<LightPageView>> GetPageTemplates(Guid workspaceId)
	{
		GetRequest request = new GetRequest() {Id = workspaceId, UserId = UserId};

		return await _pageService.GetPageTemplatesAsync(request);
	}

	[HttpGet("[action]")]
	public async Task<PageStatisticView> GetPageStatistic(Guid workspaceId)
	{
		GetRequest request = new GetRequest() {Id = workspaceId, UserId = UserId};

		return await _pageService.GetWorkspacePageStatisticsAsync(request);
	}

	#endregion

	#region Actions

	[HttpPost("[action]")]
	public async Task<Guid> CreatePage(CreatePageBlank createPageBlank)
	{
		BlankRequest<CreatePageBlank> request = new BlankRequest<CreatePageBlank>()
			{UserId = UserId, Blank = createPageBlank};

		return await _pageService.CreatePageAsync(request);
	}

	[HttpPatch("[action]")]
	public async Task<ActionResult> MovePage(MovePageBlank movePageBlank)
	{
		BlankRequest<MovePageBlank> request = new BlankRequest<MovePageBlank>()
			{UserId = UserId, Blank = movePageBlank};

		await _pageService.MovePageAsync(request);

		return Ok();
	}

	[HttpPatch("[action]")]
	public async Task<ActionResult> TogglePinPage(TogglePinPageBlank togglePageBlank)
	{
		BlankRequest<TogglePinPageBlank> request = new()
		{
			UserId = UserId,
			Blank = togglePageBlank
		};

		await _pageService.TogglePinPageAsync(request);

		return Ok();
	}

	[HttpPatch("[action]")]
	public async Task<ActionResult> ToggleArchivePage(ToggleArchivePageBlank toggleArchivePageBlank)
	{
		BlankRequest<ToggleArchivePageBlank> request = new()
		{
			UserId = UserId,
			Blank = toggleArchivePageBlank
		};

		await _pageService.ToggleArchivePageAsync(request);

		return Ok();
	}

	[HttpDelete("[action]")]
	public async Task<ActionResult> DeletePage(Guid pageId)
	{
		DeleteRequest request = new DeleteRequest() {UserId = UserId, ObjectId = pageId};

		await _pageService.DeletePageAsync(request);
		return Ok();
	}

	#endregion
}