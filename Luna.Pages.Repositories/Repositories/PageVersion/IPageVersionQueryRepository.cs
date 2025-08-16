using Luna.Pages.Models.Database.Models;

namespace Luna.Pages.Repositories.Repositories.PageVersion;

public interface IPageVersionQueryRepository
{
	Task<PageVersionDatabase?> GetPageVersionAsync(Guid pageId, int version, CancellationToken cancellationToken = default);
	Task<PageVersionDatabase?> GetLatestPageVersionAsync(Guid pageId, CancellationToken cancellationToken = default);
	Task<IEnumerable<PageVersionDatabase>> GetPageVersionHistoryAsync(Guid pageId, int skip = 0, int take = 50, CancellationToken cancellationToken = default);
	Task<int> GetPageVersionCountAsync(Guid pageId, CancellationToken cancellationToken = default);
}