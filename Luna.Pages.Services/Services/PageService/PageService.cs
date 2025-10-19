using Luna.Pages.Models.Blank.Models;
using Luna.Pages.Models.Database.Additional;
using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Models.View.Additional;
using Luna.Pages.Models.View.Models;
using Luna.Pages.Repositories.Repositories.Page.Query;
using Luna.Pages.Services.Commands.Page;
using Luna.Pages.Services.Commands.PageComment;
using Luna.Pages.Services.Commands.PageContent;
using Luna.Pages.Services.Commands.Search;
using Luna.Pages.Services.Queries.Page;
using Luna.Pages.Services.Queries.PageComment;
using Luna.Pages.Services.Queries.PageContent;
using Luna.Pages.Services.Queries.Search;
using Luna.Pages.Services.Services.WorkspacePermissionService;
using Luna.Tools.SharedModels.Models;
using Luna.Tools.SharedModels.Models.API;
using Luna.Tools.SharedModels.Models.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Services.PageService;

public class PageService : IPageService
{
	private readonly IMediator _mediator;
	private readonly IWorkspacePermissionService _workspacePermissionService;
	private readonly IPageQueryRepository _pageQueryRepository;
	private readonly ILogger<PageService> _logger;

	public PageService(
		IMediator mediator,
		IWorkspacePermissionService workspacePermissionService,
		IPageQueryRepository pageQueryRepository,
		ILogger<PageService> logger)
	{
		_mediator = mediator;
		_workspacePermissionService = workspacePermissionService;
		_pageQueryRepository = pageQueryRepository;
		_logger = logger;
	}

	public async Task<Guid> CreatePageAsync(BlankRequest<CreatePageBlank> request)
	{
		await CheckPermissionByWorkspaceIdAsync(request.Blank.WorkspaceId, request.UserId, WorkspacePermissions.Edit);

		CreatePageCommand command = new CreatePageCommand(request.UserId, request.Blank);

		Guid pageId = await _mediator.Send(command, CancellationToken.None);

		PageDomain newPageIndex = PageDomain.FromBlank(pageId, request.UserId, request.Blank);

		IndexPageCommand indexPageCommand = new IndexPageCommand(new PageVersionDomain(), newPageIndex);

		// пробуем индексировать при создании
		try
		{
			await _mediator.Send(indexPageCommand, CancellationToken.None);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}

		return pageId;
	}

	public async Task<bool> MovePageAsync(BlankRequest<MovePageBlank> request)
	{
		await CheckPermissionAsync(request.Blank.PageId, request.UserId, WorkspacePermissions.Edit);

		MovePageCommand command = new MovePageCommand(request.UserId, request.Blank);
		return await _mediator.Send(command, CancellationToken.None);
	}

	public async Task<bool> TogglePinPageAsync(BlankRequest<TogglePinPageBlank> request)
	{
		await CheckPermissionAsync(request.Blank.PageId, request.UserId, WorkspacePermissions.Edit);

		TogglePinPageCommand command = new TogglePinPageCommand(request.UserId, request.Blank);
		return await _mediator.Send(command, CancellationToken.None);
	}

	public async Task<bool> ToggleArchivePageAsync(BlankRequest<ToggleArchivePageBlank> request)
	{
		await CheckPermissionAsync(request.Blank.PageId, request.UserId, WorkspacePermissions.Edit);

		ToggleArchivePageCommand command = new ToggleArchivePageCommand(request.UserId, request.Blank);
		return await _mediator.Send(command, CancellationToken.None);
	}

	public async Task<bool> DeletePageAsync(DeleteRequest request)
	{
		await CheckPermissionAsync(request.ObjectId, request.UserId, WorkspacePermissions.Edit);

		DeletePageCommand command = new DeletePageCommand(request.ObjectId, request.UserId);
		DeletePageIndexCommand deletePageIndexCommand = new DeletePageIndexCommand(request.ObjectId.ToString());

		await _mediator.Send(deletePageIndexCommand, CancellationToken.None);
		return await _mediator.Send(command, CancellationToken.None);
	}

	public async Task<bool> UpdatePageAsync(UpdateRequest<PatchPageBlank> request)
	{
		await CheckPermissionAsync(request.ObjectId, request.UserId, WorkspacePermissions.Edit);

		UpdatePageCommand command = new UpdatePageCommand(request.ObjectId, request.Blank);

		bool updated = await _mediator.Send(command, CancellationToken.None);

		// todo: похоже на костыль
		PageFullDomain? pageFull = await GetPageFullDomainAsync(new GetRequest(){Id = request.ObjectId, UserId = request.UserId});

		if (pageFull != null)
		{
			IndexPageCommand indexPageCommand = new IndexPageCommand(pageFull.PageVersion, pageFull.Page);
			try
			{
				bool indexed = await _mediator.Send(indexPageCommand, CancellationToken.None);
				_logger.LogInformation("Indexed Page Content: {Indexed}", indexed);
			}
			catch (Exception e)
			{
				_logger.LogError("Error indexing Page Content: {Error}", e);
			}
		}

		return updated;
	}

	public async Task<bool> UpdatePageContentAsync(UpdateRequest<UpdatePageContentBlank> request)
	{
		await CheckPermissionAsync(request.ObjectId, request.UserId, WorkspacePermissions.Edit);

		UpdatePageContentCommand command = new UpdatePageContentCommand(request.ObjectId, request.UserId, request.Blank);

		bool updated = await _mediator.Send(command, CancellationToken.None);

		// todo: похоже на костыль
		PageFullDomain? pageFull = await GetPageFullDomainAsync(new GetRequest(){Id = request.ObjectId, UserId = request.UserId});

		if (pageFull != null)
		{
			IndexPageCommand indexPageCommand = new IndexPageCommand(pageFull.PageVersion, pageFull.Page);
			try
			{
				bool indexed = await _mediator.Send(indexPageCommand, CancellationToken.None);
				_logger.LogInformation("Indexed Page Content: {Indexed}", indexed);
			}
			catch (Exception e)
			{
				_logger.LogError("Error indexing Page Content: {Error}", e);
			}
		}

		return updated;
	}

	public async Task<bool> CreatePageCommentAsync(BlankRequest<CreatePageCommentBlank> request)
	{
		await CheckPermissionAsync(request.Blank.PageId, request.UserId, WorkspacePermissions.Comment);

		CreatePageCommentCommand command = new CreatePageCommentCommand(request.UserId, request.Blank);
		return await _mediator.Send(command, CancellationToken.None);
	}

	public async Task<bool> UpdatePageCommentAsync(UpdateRequest<PatchPageCommentBlank> request)
	{
		UpdatePageCommentCommand
			command = new UpdatePageCommentCommand(request.ObjectId, request.UserId, request.Blank);
		return await _mediator.Send(command, CancellationToken.None);
	}

	public async Task<bool> DeletePageCommentAsync(DeleteRequest request)
	{
		DeletePageCommentCommand command = new DeletePageCommentCommand(request.ObjectId, request.UserId);
		return await _mediator.Send(command, CancellationToken.None);
	}

	public async Task<PageView?> GetPageByIdAsync(GetRequest request)
	{
		GetPageByIdQuery query = new GetPageByIdQuery(request.Id);

		PageDomain? page = await _mediator.Send(query, CancellationToken.None);

		await CheckPermissionAsync(request.Id, request.UserId, WorkspacePermissions.View);

		return page?.ToView();
	}

	public async Task<LightPageView?> GetPageLightViewAsync(GetRequest request)
	{
		GetPageByIdQuery query = new GetPageByIdQuery(request.Id);

		PageDomain? page = await _mediator.Send(query, CancellationToken.None);

		await CheckPermissionAsync(request.Id, request.UserId, WorkspacePermissions.View);

		return page?.ToLightPageView();
	}

	public async Task<IEnumerable<PageBlockView>> GetPageBlocksAsync(GetRequest request)
	{
		GetPageBlocksQuery query = new GetPageBlocksQuery(request.Id);

		IEnumerable<PageBlockDomain> pageBlocks = await _mediator.Send(query, CancellationToken.None);

		await CheckPermissionAsync(request.Id, request.UserId, WorkspacePermissions.View);

		return pageBlocks.Select(item => item.ToView());
	}

	public async Task<PageFullView?> GetPageFullViewAsync(GetRequest request)
	{
		GetPageFullViewQuery query = new GetPageFullViewQuery(request.Id);

		PageFullDomain? pageFullDomain = await _mediator.Send(query, CancellationToken.None);

		await CheckPermissionAsync(request.Id, request.UserId, WorkspacePermissions.View);

		return pageFullDomain?.ToView();
	}

	public async Task<PageFullDomain?> GetPageFullDomainAsync(GetRequest request)
	{
		GetPageFullViewQuery query = new GetPageFullViewQuery(request.Id);

		PageFullDomain? pageFullDomain = await _mediator.Send(query, CancellationToken.None);

		await CheckPermissionAsync(request.Id, request.UserId, WorkspacePermissions.View);

		return pageFullDomain;
	}

	public async Task<IEnumerable<LightPageView>> GetWorkspacePagesAsync(GetRequest request,
		bool includeArchived = false)
	{
		GetWorkspacePagesQuery query = new GetWorkspacePagesQuery(request.Id, includeArchived);

		IEnumerable<PageDomain> pagesDomain = await _mediator.Send(query, CancellationToken.None);

		await CheckPermissionByWorkspaceIdAsync(request.Id, request.UserId, WorkspacePermissions.View);

		return pagesDomain.Select(item => item.ToLightPageView());
	}

	public async Task<IEnumerable<LightPageView>> GetPageTemplatesAsync(GetRequest request)
	{
		GetPageTemplatesQuery query = new GetPageTemplatesQuery(request.Id);

		IEnumerable<PageDomain> pagesDomain = await _mediator.Send(query, CancellationToken.None);

		await CheckPermissionByWorkspaceIdAsync(request.Id, request.UserId, WorkspacePermissions.View);

		return pagesDomain.Select(item => item.ToLightPageView());
	}

	public async Task<IEnumerable<LightPageView>> GetArchivedPagesAsync(GetRequest request)
	{
		GetArchivedPagesQuery query = new GetArchivedPagesQuery(request.Id);

		IEnumerable<PageDomain> pagesDomain = await _mediator.Send(query, CancellationToken.None);

		await CheckPermissionByWorkspaceIdAsync(request.Id, request.UserId, WorkspacePermissions.View);

		return pagesDomain.Select(item => item.ToLightPageView());
	}

	public async Task<IEnumerable<LightPageView>> SearchPagesByTitleAsync(GetRequest request, string searchTerm,
		int limit = 50)
	{
		SearchPagesByTitleQuery query = new SearchPagesByTitleQuery(searchTerm, request.Id, limit);

		IEnumerable<PageDomain> pagesDomain = await _mediator.Send(query, CancellationToken.None);

		await CheckPermissionByWorkspaceIdAsync(request.Id, request.UserId, WorkspacePermissions.View);

		return pagesDomain.Select(item => item.ToLightPageView());
	}

	public async Task<bool> PageExistsAsync(GetRequest request)
	{
		PageExistsQuery query = new PageExistsQuery(request.Id);

		await CheckPermissionByWorkspaceIdAsync(request.Id, request.UserId, WorkspacePermissions.View);

		return await _mediator.Send(query, CancellationToken.None);
	}

	public async Task<IEnumerable<PageCommentView>> GetPageCommentsAsync(GetRequest request)
	{
		GetPageCommentsQuery query = new GetPageCommentsQuery(request.Id);

		IEnumerable<PageCommentDomain> pageComments = await _mediator.Send(query, CancellationToken.None);

		await CheckPermissionByWorkspaceIdAsync(request.Id, request.UserId, WorkspacePermissions.View);

		return pageComments.Select(item => item.ToView());
	}

	public async Task<PageStatisticView> GetWorkspacePageStatisticsAsync(GetRequest request)
	{
		GetWorkspacePageStatisticsQuery query = new GetWorkspacePageStatisticsQuery(request.Id);

		await CheckPermissionByWorkspaceIdAsync(request.Id, request.UserId, WorkspacePermissions.View);

		PageStatistics statistics = await _mediator.Send(query, CancellationToken.None);

		return statistics.ToView();
	}

	// при ошибке в ES идем искать в бд
	public async Task<List<LightPageView>> SearchPagesAsync(SearchGetRequest request)
	{
		IEnumerable<LightPageView> pages;

		SearchPageQuery queryPage = new SearchPageQuery(request.Query, request.WorkspaceId, request.From, request.Size);

		await CheckPermissionByWorkspaceIdAsync(request.WorkspaceId, request.UserId, WorkspacePermissions.View);

		try
		{
			pages = await _mediator.Send(queryPage, CancellationToken.None);
		}
		catch (Exception e)
		{
			pages = await SearchPagesByTitleAsync(
				new GetRequest()
				{
					Id = request.WorkspaceId,
					UserId = request.UserId
				},
				request.Query,
				request.Size
			);
		}

		return pages.ToList();
	}

	public async Task<List<SearchPageBlockView>> SearchInBlocksAsync(SearchGetRequest request)
	{
		SearchInBlocksQuery queryPage =
			new SearchInBlocksQuery(request.Query, request.WorkspaceId, request.From, request.Size);

		await CheckPermissionByWorkspaceIdAsync(request.WorkspaceId, request.UserId, WorkspacePermissions.View);

		return await _mediator.Send(queryPage, CancellationToken.None);
	}

	private async Task CheckPermissionAsync(Guid pageId, Guid userId, string workspacePermission)
	{
		PageDatabase? page = await _pageQueryRepository.GetPageByIdAsync(pageId);

		if (page == null) throw new NotFoundException("Page not found");

		bool available = await _workspacePermissionService.HasPermissionAsync(new Guid(page.WorkspaceId), userId,
			workspacePermission);

		if (!available) throw new NotPermittedException("You do not have permission to this workspace action");
	}

	private async Task CheckPermissionByWorkspaceIdAsync(Guid workspaceId, Guid userId, string workspacePermission)
	{
		bool available = await _workspacePermissionService.HasPermissionAsync(workspaceId, userId,
			workspacePermission);

		if (!available) throw new NotPermittedException("You do not have permission to this workspace action");
	}
}