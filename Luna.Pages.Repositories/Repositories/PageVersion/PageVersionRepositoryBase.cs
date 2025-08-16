using Luna.Pages.Models.Database.Models;
using Luna.Pages.Repositories.Repositories.Page;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Luna.Pages.Repositories.Repositories.PageVersion;

public abstract class PageVersionRepositoryBase
{
	protected readonly IMongoCollection<PageVersionDatabase> PageVersionsCollection;
	protected  readonly ILogger<PageVersionRepositoryBase> Logger;

	protected PageVersionRepositoryBase(string connectionString, string databaseName, string collectionName, ILogger<PageVersionRepositoryBase> logger)
	{
		MongoClient client = new MongoClient(connectionString);
		IMongoDatabase? database = client.GetDatabase(databaseName);

		PageVersionsCollection = database.GetCollection<PageVersionDatabase>(collectionName);
		Logger = logger;
	}
}