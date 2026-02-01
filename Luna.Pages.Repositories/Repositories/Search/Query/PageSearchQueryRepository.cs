using Luna.Pages.Models.Database.Search;
using Luna.Tools.SharedModels.Models.Search;
using Microsoft.Extensions.Options;
using Nest;

namespace Luna.Pages.Repositories.Repositories.Search.Query;

public class PageSearchQueryRepository : PageSearchRepositoryBase, IPageSearchQueryRepository
{
	public PageSearchQueryRepository(IElasticClient elasticClient, IOptions<ElasticSearchSettings> settings) : base(
		elasticClient,
		settings.Value)
	{
	}

	public async Task<List<PageSearchDocument>> SearchAsync(string query, Guid workspaceId, int from = 0, int size = 10,
		CancellationToken cancellationToken = default)
	{
		ISearchResponse<PageSearchDocument>? searchResponse = await ElasticClient.SearchAsync<PageSearchDocument>(s => s
				.Index(Settings.DefaultIndex)
				.From(from)
				.Size(size)
				.Query(q => q
					.Bool(b => b
						.Must(m => m
							.MultiMatch(mm => mm
								.Query(query)
								.Fields(f => f
									.Field("title", boost: 3)
									.Field("description", boost: 2)
									.Field("content", boost: 1)
								)
								.Type(TextQueryType.BestFields)
								.Fuzziness(Fuzziness.Auto)
								.Operator(Operator.Or)
								.MinimumShouldMatch("1")
							)
						)
						.Filter(f => f
							.Term(t => t
								.Field("workspaceId")
								.Value(workspaceId.ToString())
							)
						)
					)
				)
				.Sort(ss => ss.Descending(SortSpecialField.Score)),
			cancellationToken
		);

		return searchResponse.IsValid ? searchResponse.Documents.ToList() : new List<PageSearchDocument>();
	}

	public async Task<List<PageBlockSearchContent>> SearchInBlocksAsync(string query, Guid workspaceId, int from = 0,
		int size = 10,
		CancellationToken cancellationToken = default)
	{
		ISearchResponse<PageSearchDocument>? searchResponse = await ElasticClient.SearchAsync<PageSearchDocument>(s => s
				.Index(Settings.DefaultIndex)
				.From(from)
				.Size(size)
				.Query(q => q
					.Bool(b => b
						.Must(m => m
							.Nested(n => n
								.Path("blocks")
								.Query(nq => nq
									.Match(mq => mq
										.Field("blocks.content")
										.Query(query)
										.Fuzziness(Fuzziness.Auto)
									)
								)
								.InnerHits(ih => ih
									.Size(10)
									.Name("matched_blocks")
								)
							)
						)
						.Filter(f => f
							.Term(t => t
								.Field("workspaceId")
								.Value(workspaceId.ToString())
							)
						)
					)
				),
			cancellationToken
		);

		if (!searchResponse.IsValid)
		{
			return new List<PageBlockSearchContent>();
		}

		List<PageBlockSearchContent> results = new List<PageBlockSearchContent>();
		HashSet<string> seen = new HashSet<string>();

		foreach (IHit<PageSearchDocument>? hit in searchResponse.Hits)
		{
			if (hit.InnerHits?.TryGetValue("matched_blocks", out InnerHitsResult? innerHits) == true)
			{
				foreach (IHit<ILazyDocument>? innerHit in innerHits.Hits.Hits)
				{
					PageBlockSearchContent? block = innerHit.Source.As<PageBlockSearchContent>();

					if (block != null)
					{
						string key = $"{block.PageId}:{block.BlockId}";
						if (seen.Add(key))
						{
							results.Add(block);
						}
					}
				}
			}
			else if (hit.Source?.Blocks != null)
			{
				foreach (PageBlockSearchContent block in hit.Source.Blocks)
				{
					if (string.IsNullOrWhiteSpace(block.Content)) continue;
					if (!block.Content.Contains(query, StringComparison.OrdinalIgnoreCase)) continue;

					string key = $"{block.PageId}:{block.BlockId}";
					if (seen.Add(key))
					{
						results.Add(block);
					}
				}
			}
		}

		return results;
	}

	public async Task<bool> IndexExistsAsync(CancellationToken cancellationToken = default)
	{
		ExistsResponse? response =
			await ElasticClient.Indices.ExistsAsync(Settings.DefaultIndex, ct: cancellationToken);
		return response.Exists;
	}
}