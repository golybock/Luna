using Luna.Tools.SharedModels.Models.Search;
using Nest;

namespace Luna.Pages.Repositories.Repositories.Search;

public class PageSearchRepositoryBase
{
	protected readonly IElasticClient ElasticClient;
	protected readonly ElasticSearchSettings Settings;

	public PageSearchRepositoryBase(IElasticClient elasticClient, ElasticSearchSettings settings)
	{
		ElasticClient = elasticClient;
		Settings = settings;
	}
}