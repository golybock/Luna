using System.Text.Json;
using Elasticsearch.Net;
using Luna.Pages.Models.Database.Search;
using Luna.Tools.SharedModels.Models.Search;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;

namespace Luna.Pages.Repositories.Repositories.Search.Command;

public class PageSearchCommandRepository : PageSearchRepositoryBase, IPageSearchCommandRepository
{
	private readonly ILogger<PageSearchCommandRepository> _logger;

	public PageSearchCommandRepository(IElasticClient elasticClient, IOptions<ElasticSearchSettings> settings, ILogger<PageSearchCommandRepository> logger) : base(elasticClient, settings.Value)
	{
		_logger = logger;
	}

	public async Task<bool> IndexPageAsync(PageSearchDocument document, CancellationToken cancellationToken = default)
	{
		IndexResponse? response = await ElasticClient.IndexAsync(document, idx => idx
			.Index(Settings.DefaultIndex)
			.Id(document.PageId)
			.Refresh(Refresh.True), cancellationToken);

		if (!response.IsValid)
		{
			_logger.LogError("Error: {Error}",  response.OriginalException.Message);
		}

		return response.IsValid;
	}

	public async Task<bool> DeletePageAsync(string pageId, CancellationToken cancellationToken = default)
	{
		ISearchResponse<PageSearchDocument>? searchResponse = await ElasticClient
			.SearchAsync<PageSearchDocument>(s => s
					.Index(Settings.DefaultIndex)
					.Query(q => q
						.Term(t => t
							.Field(f => f.PageId)
							.Value(pageId)
						)
					),
				cancellationToken
			);

		if (!searchResponse.IsValid || !searchResponse.Documents.Any())
			return false;

		BulkResponse? bulkResponse = await ElasticClient.BulkAsync(b => b
				.Index(Settings.DefaultIndex)
				.DeleteMany(searchResponse.Documents)
				.Refresh(Refresh.True), cancellationToken
		);

		return bulkResponse.IsValid;
	}

	public async Task<bool> CreateIndexAsync(CancellationToken cancellationToken = default)
	{
		CreateIndexResponse? response = await ElasticClient.Indices.CreateAsync(Settings.DefaultIndex, c => c
			.Settings(s => s
				.NumberOfShards(Settings.NumberOfShards)
				.NumberOfReplicas(Settings.NumberOfReplicas)
			)
			.Map<PageSearchDocument>(m => m
				.AutoMap()
				.Properties(p => p
					.Text(t => t.Name(n => n.Title).Analyzer("standard"))
					.Text(t => t.Name(n => n.Description).Analyzer("standard"))
					.Text(t => t.Name(n => n.Content).Analyzer("standard"))
					.Keyword(k => k.Name(n => n.PageId))
					.Keyword(k => k.Name(n => n.WorkspaceId))
					.Nested<PageBlockSearchContent>(n => n
						.Name(nn => nn.Blocks)
						.Properties(bp => bp
							.Keyword(k => k.Name(b => b.BlockId))
							.Keyword(k => k.Name(b => b.PageId))
							.Keyword(k => k.Name(b => b.Type))
							.Text(t => t.Name(b => b.Content).Analyzer("standard"))
						)
					)
				)
			), cancellationToken);

		return response.IsValid;
	}

	public async Task<bool> DeleteIndexAsync(CancellationToken cancellationToken = default)
	{
		DeleteIndexResponse? response =
			await ElasticClient.Indices.DeleteAsync(Settings.DefaultIndex, ct: cancellationToken);
		return response.IsValid;
	}
}