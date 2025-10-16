using Luna.Pages.Models.Database.Additional;
using Luna.Pages.Models.Database.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Luna.Pages.Repositories.Repositories.Page.Query;

public class PageQueryRepository : PageRepositoryBase, IPageQueryRepository
{
	public PageQueryRepository(string connectionString, string databaseName, string collectionName,
		ILogger<PageQueryRepository> logger)
		: base(connectionString, databaseName, collectionName, logger)
	{
	}

	public async Task<PageDatabase?> GetPageByIdAsync(Guid pageId, CancellationToken cancellationToken = default)
	{
		FilterDefinition<PageDatabase> filter = Builders<PageDatabase>.Filter.Eq("_id", pageId.ToString());

		return await PagesCollection.Find<PageDatabase>(filter)
			.FirstOrDefaultAsync(cancellationToken: cancellationToken);
	}

	public async Task<IEnumerable<PageDatabase>> GetChildPagesAsync(Guid parentId, bool includeArchived = false,
		CancellationToken cancellationToken = default)
	{
		FilterDefinition<PageDatabase> filter =
			Builders<PageDatabase>.Filter.Eq(nameof(PageDatabase.ParentId), parentId);

		return await PagesCollection.Find<PageDatabase>(filter).ToListAsync(cancellationToken: cancellationToken);
	}

	public async Task<IEnumerable<PageDatabase>> GetRootPagesAsync(Guid workspaceId, bool includeArchived = false,
		CancellationToken cancellationToken = default)
	{
		FilterDefinition<PageDatabase> filter = Builders<PageDatabase>.Filter.And(
			Builders<PageDatabase>.Filter.Eq(nameof(PageDatabase.ParentId), BsonNull.Value),
			Builders<PageDatabase>.Filter.Eq(nameof(PageDatabase.WorkspaceId), workspaceId)
		);

		return await PagesCollection.Find<PageDatabase>(filter).ToListAsync(cancellationToken: cancellationToken);
	}

	public async Task<IEnumerable<PageDatabase>> GetWorkspacePagesAsync(Guid workspaceId, bool includeArchived = false,
		CancellationToken cancellationToken = default)
	{
		FilterDefinitionBuilder<PageDatabase>? filterBuilder = Builders<PageDatabase>.Filter;
		FilterDefinition<PageDatabase>? filter = filterBuilder.And(
			filterBuilder.Eq(nameof(PageDatabase.WorkspaceId), workspaceId),
			filterBuilder.Eq(nameof(PageDatabase.DeletedAt), BsonNull.Value)
		);

		if (!includeArchived)
		{
			filter = filterBuilder.And(filter, filterBuilder.Eq(nameof(PageDatabase.ArchivedAt), BsonNull.Value));
		}

		return await PagesCollection
			.Find(filter)
			.SortBy(x => x.ParentId)
			.ThenBy(x => x.Index)
			.ToListAsync(cancellationToken);
	}

	public async Task<IEnumerable<PageDatabase>> GetPagesByIdAsync(IEnumerable<Guid> pageIds, CancellationToken cancellationToken = default)
	{
		FilterDefinitionBuilder<PageDatabase>? filterBuilder = Builders<PageDatabase>.Filter;
		FilterDefinition<PageDatabase>? filter = filterBuilder.And(
			filterBuilder.In(nameof(PageDatabase.Id), pageIds),
			filterBuilder.Eq(nameof(PageDatabase.DeletedAt), BsonNull.Value)
		);

		return await PagesCollection
			.Find(filter)
			.SortBy(x => x.ParentId)
			.ThenBy(x => x.Index)
			.ToListAsync(cancellationToken);
	}

	public async Task<IEnumerable<PageDatabase>> GetPageTemplatesAsync(Guid workspaceId,
		CancellationToken cancellationToken = default)
	{
		FilterDefinitionBuilder<PageDatabase>? filterBuilder = Builders<PageDatabase>.Filter;
		FilterDefinition<PageDatabase>? filter = filterBuilder.And(
			filterBuilder.Eq(nameof(PageDatabase.WorkspaceId), workspaceId),
			filterBuilder.Eq(nameof(PageDatabase.DeletedAt), BsonNull.Value),
			filterBuilder.Eq(nameof(PageDatabase.IsTemplate), true)
		);

		return await PagesCollection
			.Find(filter)
			.SortBy(x => x.ParentId)
			.ThenBy(x => x.Index ?? 0)
			.ToListAsync(cancellationToken);
	}

	public async Task<IEnumerable<PageDatabase>> GetArchivedPagesAsync(Guid workspaceId,
		CancellationToken cancellationToken = default)
	{
		FilterDefinitionBuilder<PageDatabase>? filterBuilder = Builders<PageDatabase>.Filter;
		FilterDefinition<PageDatabase>? filter = filterBuilder.And(
			filterBuilder.Eq(nameof(PageDatabase.WorkspaceId), workspaceId),
			filterBuilder.Eq(nameof(PageDatabase.DeletedAt), BsonNull.Value),
			filterBuilder.Not(nameof(PageDatabase.ArchivedAt))
		);

		return await PagesCollection
			.Find(filter)
			.SortBy(x => x.ParentId)
			.ThenBy(x => x.Index ?? 0)
			.ToListAsync(cancellationToken);
	}

	public async Task<IEnumerable<PageDatabase>> SearchPagesByTitleAsync(string searchTerm, Guid workspaceId,
		int limit = 50, CancellationToken cancellationToken = default)
	{
		FilterDefinitionBuilder<PageDatabase>? filterBuilder = Builders<PageDatabase>.Filter;
		FilterDefinition<PageDatabase>? filter = filterBuilder.And(
			filterBuilder.Eq(nameof(PageDatabase.WorkspaceId), workspaceId),
			filterBuilder.Eq(nameof(PageDatabase.DeletedAt), BsonNull.Value),
			filterBuilder.Regex(nameof(PageDatabase.Title), new BsonRegularExpression(searchTerm, "i"))
		);

		return await PagesCollection
			.Find(filter)
			.SortBy(x => x.ParentId)
			.Limit(50)
			.ToListAsync(cancellationToken);
	}

	public async Task<bool> PageExistsAsync(Guid pageId, CancellationToken cancellationToken = default)
	{
		FilterDefinitionBuilder<PageDatabase> filterBuilder = Builders<PageDatabase>.Filter;
		FilterDefinition<PageDatabase> filter = filterBuilder.And(
			filterBuilder.Eq(nameof(PageDatabase.Id), pageId),
			filterBuilder.Eq(nameof(PageDatabase.DeletedAt), BsonNull.Value)
		);

		long count = await PagesCollection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
		return count > 0;
	}

	public async Task<PageStatistics> GetWorkspacePageStatisticsAsync(Guid workspaceId,
		CancellationToken cancellationToken = default)
	{
		FilterDefinitionBuilder<PageDatabase> fb = Builders<PageDatabase>.Filter;
		FilterDefinition<PageDatabase> inWorkspace = fb.Eq(nameof(PageDatabase.WorkspaceId), workspaceId);

		// Total pages in workspace (including deleted)
		Task<long> totalTask = PagesCollection.CountDocumentsAsync(inWorkspace, cancellationToken: cancellationToken);

		// Deleted pages
		Task<long> deletedTask = PagesCollection.CountDocumentsAsync(
			fb.And(inWorkspace, fb.Ne(nameof(PageDatabase.DeletedAt), BsonNull.Value)),
			cancellationToken: cancellationToken);

		// Archived pages (not deleted)
		Task<long> archivedTask = PagesCollection.CountDocumentsAsync(
			fb.And(
				inWorkspace,
				fb.Eq(nameof(PageDatabase.DeletedAt), BsonNull.Value),
				fb.Ne(nameof(PageDatabase.ArchivedAt), BsonNull.Value)
			),
			cancellationToken: cancellationToken);

		// Active pages (not deleted and not archived)
		Task<long> activeTask = PagesCollection.CountDocumentsAsync(
			fb.And(
				inWorkspace,
				fb.Eq(nameof(PageDatabase.DeletedAt), BsonNull.Value),
				fb.Eq(nameof(PageDatabase.ArchivedAt), BsonNull.Value)
			),
			cancellationToken: cancellationToken);

		// Pinned pages (not deleted)
		Task<long> pinnedTask = PagesCollection.CountDocumentsAsync(
			fb.And(
				inWorkspace,
				fb.Eq(nameof(PageDatabase.DeletedAt), BsonNull.Value),
				fb.Eq(nameof(PageDatabase.Pinned), true)
			),
			cancellationToken: cancellationToken);

		// Templates (not deleted)
		Task<long> templatesTask = PagesCollection.CountDocumentsAsync(
			fb.And(
				inWorkspace,
				fb.Eq(nameof(PageDatabase.DeletedAt), BsonNull.Value),
				fb.Eq(nameof(PageDatabase.IsTemplate), true)
			),
			cancellationToken: cancellationToken);

		await Task.WhenAll(totalTask, activeTask, archivedTask, deletedTask, pinnedTask, templatesTask);

		return new PageStatistics
		{
			TotalPages = (int) totalTask.Result,
			ArchivedPages = (int) archivedTask.Result,
			DeletedPages = (int) deletedTask.Result,
			PinnedPages = (int) pinnedTask.Result,
			Templates = (int) templatesTask.Result
		};
	}
}