using Luna.Pages.Models.Database.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Luna.Pages.Repositories.Repositories.PageVersion;

public abstract class PageVersionRepositoryBase
{
	protected readonly IMongoCollection<PageVersionDatabase> PageVersionsCollection;
	protected  readonly ILogger<PageVersionRepositoryBase> Logger;

	protected PageVersionRepositoryBase(IMongoClient client, string databaseName, string collectionName, ILogger<PageVersionRepositoryBase> logger)
	{
		IMongoDatabase? database = client.GetDatabase(databaseName);

		PageVersionsCollection = database.GetCollection<PageVersionDatabase>(collectionName);
		Logger = logger;
	}
}