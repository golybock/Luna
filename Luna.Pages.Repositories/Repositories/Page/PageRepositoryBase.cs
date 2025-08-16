using Luna.Pages.Models.Database.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Luna.Pages.Repositories.Repositories.Page;

public abstract class PageRepositoryBase
{
	protected readonly IMongoCollection<PageDatabase> PagesCollection;
	protected  readonly ILogger<PageRepositoryBase> Logger;

	protected PageRepositoryBase(string connectionString, string databaseName, string collectionName, ILogger<PageRepositoryBase> logger)
	{
		MongoClient client = new MongoClient(connectionString);
		IMongoDatabase? database = client.GetDatabase(databaseName);

		PagesCollection = database.GetCollection<PageDatabase>(collectionName);
		Logger = logger;
	}
}