namespace Luna.Tools.SharedModels.Models.Search;

public class ElasticSearchSettings
{
	public string Url { get; set; } = null!;
	public string DefaultIndex { get; set; } = null!;
	public int NumberOfShards { get; set; } = 1;
	public int NumberOfReplicas { get; set; } = 0;
}