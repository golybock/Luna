using Luna.Pages.Models.Database.Search;

namespace Luna.Pages.Repositories.Repositories.Search.Command;

public interface IPageSearchCommandRepository
{
	Task<bool> IndexPageAsync(PageSearchDocument document, CancellationToken cancellationToken = default);
	Task<bool> DeletePageAsync(string pageId, CancellationToken cancellationToken = default);
	Task<bool> CreateIndexAsync(CancellationToken cancellationToken = default);
	Task<bool> DeleteIndexAsync(CancellationToken cancellationToken = default);
}