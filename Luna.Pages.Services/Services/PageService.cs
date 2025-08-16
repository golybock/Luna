using Luna.Pages.Models.Blank.Models;
using Luna.Pages.Models.Database.Additional;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Models.View.Additional;
using Luna.Pages.Models.View.Models;
using Luna.Pages.Services.Commands.Page;
using Luna.Pages.Services.Commands.PageComment;
using Luna.Pages.Services.Commands.PageContent;
using Luna.Pages.Services.Queries.Page;
using Luna.Pages.Services.Queries.PageComment;
using Luna.Pages.Services.Queries.PageContent;
using Luna.Tools.SharedModels.Models.API;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.Services;

public class PageService : IPageService
{
	private readonly IMediator _mediator;
	// private readonly ILogger _logger;

	public PageService(IMediator mediator)
	{
		_mediator = mediator;
		// _logger = logger;
	}

	public async Task<Guid> CreatePageAsync(BlankRequest<CreatePageBlank> request)
	{
		try
		{
			CreatePageCommand command = new CreatePageCommand(request.UserId, request.Blank);
			return await _mediator.Send(command, CancellationToken.None);
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<bool> MovePageAsync(BlankRequest<MovePageBlank> request)
	{
		try
		{
			MovePageCommand command = new MovePageCommand(request.UserId, request.Blank);
			return await _mediator.Send(command, CancellationToken.None);
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<bool> TogglePinPageAsync(BlankRequest<TogglePinPageBlank> request)
	{
		try
		{
			TogglePinPageCommand command = new TogglePinPageCommand(request.UserId, request.Blank);
			return await _mediator.Send(command, CancellationToken.None);
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<bool> ToggleArchivePageAsync(BlankRequest<ToggleArchivePageBlank> request)
	{
		try
		{
			ToggleArchivePageCommand command = new ToggleArchivePageCommand(request.UserId, request.Blank);
			return await _mediator.Send(command, CancellationToken.None);
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<bool> DeletePageAsync(DeleteRequest request)
	{
		try
		{
			DeletePageCommand command = new DeletePageCommand(request.ObjectId, request.UserId);
			return await _mediator.Send(command, CancellationToken.None);
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<bool> UpdatePageAsync(UpdateRequest<PatchPageBlank> request)
	{
		try
		{
			UpdatePageCommand command = new UpdatePageCommand(request.ObjectId, request.Blank);
			return await _mediator.Send(command, CancellationToken.None);
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<bool> UpdatePageContentAsync(UpdateRequest<UpdatePageContentBlank> request)
	{
		try
		{
			UpdatePageContentCommand command =
				new UpdatePageContentCommand(request.ObjectId, request.UserId, request.Blank);
			return await _mediator.Send(command, CancellationToken.None);
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<bool> CreatePageCommentAsync(BlankRequest<CreatePageCommentBlank> request)
	{
		try
		{
			CreatePageCommentCommand command = new CreatePageCommentCommand(request.UserId, request.Blank);
			return await _mediator.Send(command, CancellationToken.None);
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<bool> UpdatePageCommentAsync(UpdateRequest<PatchPageCommentBlank> request)
	{
		try
		{
			UpdatePageCommentCommand command =
				new UpdatePageCommentCommand(request.ObjectId, request.UserId, request.Blank);
			return await _mediator.Send(command, CancellationToken.None);
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<bool> DeletePageCommentAsync(DeleteRequest request)
	{
		try
		{
			DeletePageCommentCommand command = new DeletePageCommentCommand(request.ObjectId, request.UserId);
			return await _mediator.Send(command, CancellationToken.None);
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<PageView?> GetPageByIdAsync(GetRequest request)
	{
		try
		{
			GetPageByIdQuery query = new GetPageByIdQuery(request.Id);

			PageDomain? page = await _mediator.Send(query, CancellationToken.None);

			return page?.ToView();
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<LightPageView?> GetPageLightViewAsync(GetRequest request)
	{
		try
		{
			GetPageByIdQuery query = new GetPageByIdQuery(request.Id);

			PageDomain? page = await _mediator.Send(query, CancellationToken.None);

			return page?.ToLightPageView();
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<IEnumerable<PageBlockView>> GetPageBlocksAsync(GetRequest request)
	{
		try
		{
			GetPageBlocksQuery query = new GetPageBlocksQuery(request.Id);

			IEnumerable<PageBlockDomain> pageBlocks = await _mediator.Send(query, CancellationToken.None);

			return pageBlocks.Select(item => item.ToView());
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<PageFullView?> GetPageFullViewAsync(GetRequest request)
	{
		try
		{
			GetPageFullViewQuery query = new GetPageFullViewQuery(request.Id);

			PageFullDomain? pageFullDomain = await _mediator.Send(query, CancellationToken.None);

			return pageFullDomain?.ToView();
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<IEnumerable<LightPageView>> GetWorkspacePagesAsync(GetRequest request, bool includeArchived = false)
	{
		try
		{
			GetWorkspacePagesQuery query = new GetWorkspacePagesQuery(request.Id, includeArchived);

			IEnumerable<PageDomain> pagesDomain = await _mediator.Send(query, CancellationToken.None);

			return pagesDomain.Select(item => item.ToLightPageView());
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<IEnumerable<LightPageView>> GetPageTemplatesAsync(GetRequest request)
	{
		try
		{
			GetPageTemplatesQuery query = new GetPageTemplatesQuery(request.Id);

			IEnumerable<PageDomain> pagesDomain = await _mediator.Send(query, CancellationToken.None);

			return pagesDomain.Select(item => item.ToLightPageView());
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<IEnumerable<LightPageView>> GetArchivedPagesAsync(GetRequest request)
	{
		try
		{
			GetArchivedPagesQuery query = new GetArchivedPagesQuery(request.Id);

			IEnumerable<PageDomain> pagesDomain = await _mediator.Send(query, CancellationToken.None);

			return pagesDomain.Select(item => item.ToLightPageView());
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<IEnumerable<LightPageView>> SearchPagesByTitleAsync(GetRequest request, string searchTerm, int limit = 50)
	{
		try
		{
			SearchPagesByTitleQuery query = new SearchPagesByTitleQuery(searchTerm, request.Id, limit);

			IEnumerable<PageDomain> pagesDomain = await _mediator.Send(query, CancellationToken.None);

			return pagesDomain.Select(item => item.ToLightPageView());
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<bool> PageExistsAsync(GetRequest request)
	{
		try
		{
			PageExistsQuery query = new PageExistsQuery(request.Id);

			return await _mediator.Send(query, CancellationToken.None);
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<IEnumerable<PageCommentView>> GetPageCommentsAsync(GetRequest request)
	{
		try
		{
			GetPageCommentsQuery query = new GetPageCommentsQuery(request.Id);

			IEnumerable<PageCommentDomain> pageComments = await _mediator.Send(query, CancellationToken.None);

			return pageComments.Select(item => item.ToView());
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}

	public async Task<PageStatisticView> GetWorkspacePageStatisticsAsync(GetRequest request)
	{
		try
		{
			GetWorkspacePageStatisticsQuery query = new GetWorkspacePageStatisticsQuery(request.Id);

			PageStatistics statistics = await _mediator.Send(query, CancellationToken.None);

			return statistics.ToView();
		}
		catch (Exception ex)
		{
			//_logger.LogError("{Message}", ex.Message);
			throw;
		}
	}
}