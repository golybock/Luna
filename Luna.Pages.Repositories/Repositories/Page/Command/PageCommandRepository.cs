using Luna.Pages.Models.Database.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Luna.Pages.Repositories.Repositories.Page.Command;

public class PageCommandRepository : PageRepositoryBase, IPageCommandRepository
{
	public PageCommandRepository(string connectionString, string databaseName, string collectionName, ILogger<PageCommandRepository> logger)
		: base(connectionString, databaseName, collectionName, logger) { }

	public async Task CreatePageAsync(PageDatabase page, CancellationToken cancellationToken = default)
	{
		await PagesCollection.InsertOneAsync(page, cancellationToken: cancellationToken);
	}

	public async Task<bool> PatchPageAsync(Guid pageId, Dictionary<string, object?> updates, CancellationToken cancellationToken = default)
	{
		if (!updates.Any())
			return true;

		FilterDefinition<PageDatabase> filter = Filters.Filters.ActivePageById(pageId.ToString());
		UpdateDefinition<PageDatabase>? updateBuilder = Builders<PageDatabase>.Update.Set(p => p.UpdatedAt, DateTime.UtcNow);

		// исключения
		HashSet<string> prohibitedFields =
		[
			nameof(PageDatabase.Id),
			nameof(PageDatabase.CreatedAt),
			nameof(PageDatabase.LatestVersion)
		];

		foreach (var (key, value) in updates.Where(u => !prohibitedFields.Contains(u.Key)))
		{
			updateBuilder = updateBuilder.Set(key, value);
		}

		UpdateResult? result = await PagesCollection.UpdateOneAsync(filter, updateBuilder, cancellationToken: cancellationToken);

		return result.ModifiedCount > 0;
	}

	public async Task<bool> DeletePageAsync(Guid pageId, Guid deletedBy, CancellationToken cancellationToken = default)
	{
		FilterDefinition<PageDatabase> filter = Builders<PageDatabase>.Filter.Eq("_id", pageId.ToString());
		UpdateDefinition<PageDatabase> update = Builders<PageDatabase>.Update
			.Set(p => p.UpdatedAt, DateTime.UtcNow)
			.Set(p => p.DeletedAt, DateTime.UtcNow);

		UpdateResult? result = await PagesCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

		return result.ModifiedCount > 0;
	}
}