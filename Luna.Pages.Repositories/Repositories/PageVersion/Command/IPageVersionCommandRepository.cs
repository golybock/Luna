using Luna.Pages.Models.Database.Models;

namespace Luna.Pages.Repositories.Repositories.PageVersion.Command;

public interface IPageVersionCommandRepository
{
	Task<bool> CreatePageVersionAsync(PageVersionDatabase versionDatabase, CancellationToken cancellationToken = default);
}