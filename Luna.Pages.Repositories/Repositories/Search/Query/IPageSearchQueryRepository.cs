using Luna.Pages.Models.Database.Search;

namespace Luna.Pages.Repositories.Repositories.Search.Query;

public interface IPageSearchQueryRepository
{
	Task<List<PageSearchDocument>> SearchAsync(string query, Guid workspaceId, int from = 0, int size = 10, CancellationToken cancellationToken = default);
	Task<List<PageBlockSearchContent>> SearchInBlocksAsync(string query, Guid workspaceId, int from = 0, int size = 10, CancellationToken cancellationToken = default);
	Task<bool> IndexExistsAsync(CancellationToken cancellationToken = default);
}