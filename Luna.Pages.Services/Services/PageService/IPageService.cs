using Luna.Pages.Models.Blank.Models;
using Luna.Pages.Models.View.Additional;
using Luna.Pages.Models.View.Models;
using Luna.Tools.SharedModels.Models.API;

namespace Luna.Pages.Services.Services.PageService;

public interface IPageService
{
	// Commands
	Task<Guid> CreatePageAsync(BlankRequest<CreatePageBlank> request);
	Task<bool> MovePageAsync(BlankRequest<MovePageBlank> request);
	Task<bool> TogglePinPageAsync(BlankRequest<TogglePinPageBlank> request);
	Task<bool> ToggleArchivePageAsync(BlankRequest<ToggleArchivePageBlank> request);
	Task<bool> DeletePageAsync(DeleteRequest request);
	Task<bool> UpdatePageAsync(UpdateRequest<PatchPageBlank> request);
	Task<bool> UpdatePageContentAsync(UpdateRequest<UpdatePageContentBlank> request);
	Task<bool> CreatePageCommentAsync(BlankRequest<CreatePageCommentBlank> request);
	Task<bool> UpdatePageCommentAsync(UpdateRequest<PatchPageCommentBlank> request);
	Task<bool> DeletePageCommentAsync(DeleteRequest request);

	// Queries
	Task<PageView?> GetPageByIdAsync(GetRequest request);
	Task<LightPageView?> GetPageLightViewAsync(GetRequest request);
	Task<PageFullView?> GetPageFullViewAsync(GetRequest request);

	Task<IEnumerable<LightPageView>> GetWorkspacePagesAsync(GetRequest request, bool includeArchived = false);

	Task<IEnumerable<LightPageView>> GetPageTemplatesAsync(GetRequest request);

	Task<IEnumerable<LightPageView>> GetArchivedPagesAsync(GetRequest request);

	Task<IEnumerable<LightPageView>> SearchPagesByTitleAsync(GetRequest request, string searchTerm, int limit = 50);

	Task<bool> PageExistsAsync(GetRequest request);
	Task<IEnumerable<PageCommentView>> GetPageCommentsAsync(GetRequest request);

	Task<PageStatisticView> GetWorkspacePageStatisticsAsync(GetRequest request);

	// Search queries
	Task<List<LightPageView>> SearchPagesAsync(SearchGetRequest request);
	Task<List<SearchPageBlockView>> SearchInBlocksAsync(SearchGetRequest request);
}