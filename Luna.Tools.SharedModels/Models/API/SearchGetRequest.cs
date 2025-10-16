namespace Luna.Tools.SharedModels.Models.API;

public class SearchGetRequest
{
	public Guid UserId { get; set; }
	public Guid WorkspaceId { get; set; }
	public string Query { get; set; } = null!;
	public int From { get; set; } = 0;
	public int Size { get; set; } = 10;
}