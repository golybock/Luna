using Luna.Pages.Models.Database.Models;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Repositories.Repositories.PageVersion.Command;

public class PageVersionCommandRepository : PageVersionRepositoryBase, IPageVersionCommandRepository
{
	public PageVersionCommandRepository(string connectionString, string databaseName, string collectionName, ILogger<PageVersionCommandRepository> logger) : base(connectionString, databaseName, collectionName, logger)
	{
	}

	public async Task<bool> CreatePageVersionAsync(PageVersionDatabase versionDatabase, CancellationToken cancellationToken = default)
	{
		await PageVersionsCollection.InsertOneAsync(versionDatabase, cancellationToken: cancellationToken);
		return true;
	}
}