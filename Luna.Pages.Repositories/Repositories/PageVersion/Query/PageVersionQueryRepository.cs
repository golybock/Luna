using Luna.Pages.Models.Database.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Luna.Pages.Repositories.Repositories.PageVersion.Query;

public class PageVersionQueryRepository : PageVersionRepositoryBase, IPageVersionQueryRepository
{
	public PageVersionQueryRepository(string connectionString, string databaseName, string collectionName,
		ILogger<PageVersionQueryRepository> logger) : base(connectionString, databaseName, collectionName, logger)
	{
	}

	public async Task<PageVersionDatabase?> GetPageVersionAsync(Guid pageId, int version,
		CancellationToken cancellationToken = default)
	{
		FilterDefinition<PageVersionDatabase> filter = Builders<PageVersionDatabase>.Filter.And(
			Builders<PageVersionDatabase>.Filter.Eq(nameof(PageVersionDatabase.PageId), pageId),
			Builders<PageVersionDatabase>.Filter.Eq(nameof(PageVersionDatabase.Version), version)
		);

		return await PageVersionsCollection
			.Find(filter)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<PageVersionDatabase?> GetLatestPageVersionAsync(Guid pageId,
		CancellationToken cancellationToken = default)
	{
		FilterDefinition<PageVersionDatabase> filter = Builders<PageVersionDatabase>.Filter.And(
			Builders<PageVersionDatabase>.Filter.Eq(nameof(PageVersionDatabase.PageId), pageId.ToString())
		);

		return await PageVersionsCollection
			.Find(filter)
			.SortByDescending(item => item.Version)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IEnumerable<PageVersionDatabase>> GetPageVersionHistoryAsync(Guid pageId, int skip = 0,
		int take = 50,
		CancellationToken cancellationToken = default)
	{
		FilterDefinition<PageVersionDatabase> filter = Builders<PageVersionDatabase>.Filter
			.And(
				Builders<PageVersionDatabase>.Filter.Eq(nameof(PageVersionDatabase.PageId), pageId)
			);

		return await PageVersionsCollection
			.Find(filter)
			.Skip(skip)
			.Limit(take)
			.SortByDescending(item => item.Version)
			.ToListAsync(cancellationToken);
	}

	public async Task<int> GetPageVersionCountAsync(Guid pageId, CancellationToken cancellationToken = default)
	{
		FilterDefinition<PageVersionDatabase> filter = Builders<PageVersionDatabase>.Filter.And(
			Builders<PageVersionDatabase>.Filter.Eq(nameof(PageVersionDatabase.PageId), pageId)
		);

		long count = await PageVersionsCollection
			.Find(filter)
			.SortByDescending(item => item.Version)
			.CountDocumentsAsync(cancellationToken);

		return Int32.Parse(count.ToString());
	}
}