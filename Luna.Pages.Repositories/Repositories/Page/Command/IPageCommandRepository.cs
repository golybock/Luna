using Luna.Pages.Models.Database.Models;

namespace Luna.Pages.Repositories.Repositories.Page.Command;

public interface IPageCommandRepository
{
	Task CreatePageAsync(PageDatabase page, CancellationToken cancellationToken = default);
	Task<bool> PatchPageAsync(Guid pageId, Dictionary<string, object?> updates, CancellationToken cancellationToken = default);

	Task<bool> DeletePageAsync(Guid pageId, Guid deletedBy, CancellationToken cancellationToken = default);
}