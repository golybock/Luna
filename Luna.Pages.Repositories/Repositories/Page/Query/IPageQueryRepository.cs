using Luna.Pages.Models.Database.Additional;
using Luna.Pages.Models.Database.Models;

namespace Luna.Pages.Repositories.Repositories.Page.Query;

public interface IPageQueryRepository
{
	Task<PageDatabase?> GetPageByIdAsync(Guid pageId, CancellationToken cancellationToken = default);

	Task<IEnumerable<PageDatabase>> GetChildPagesAsync(Guid parentId, bool includeArchived = false, CancellationToken cancellationToken = default);

	Task<IEnumerable<PageDatabase>> GetRootPagesAsync(Guid workspaceId, bool includeArchived = false, CancellationToken cancellationToken = default);

	Task<IEnumerable<PageDatabase>> GetWorkspacePagesAsync(Guid workspaceId, bool includeArchived = false, CancellationToken cancellationToken = default);
	Task<IEnumerable<PageDatabase>> GetPagesByIdAsync(IEnumerable<Guid> pageIds, CancellationToken cancellationToken = default);

	Task<IEnumerable<PageDatabase>> GetPageTemplatesAsync(Guid workspaceId, CancellationToken cancellationToken = default);

	Task<IEnumerable<PageDatabase>> GetArchivedPagesAsync(Guid workspaceId, CancellationToken cancellationToken = default);

	Task<IEnumerable<PageDatabase>> SearchPagesByTitleAsync(string searchTerm, Guid workspaceId, int limit = 50, CancellationToken cancellationToken = default);

	Task<bool> PageExistsAsync(Guid pageId, CancellationToken cancellationToken = default);

	Task<PageStatistics> GetWorkspacePageStatisticsAsync(Guid workspaceId, CancellationToken cancellationToken = default);
}