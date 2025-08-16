using MongoDB.Bson;

namespace Luna.Pages.Models.Blank.Models;

public class PatchPageBlank
{
	public string? Title { get; set; }
	public string? Description { get; set; }
	public string? Icon { get; set; }
	public string? Cover { get; set; }
	public string? Emoji { get; set; }
	public string? Type { get; set; }
	public bool? IsTemplate { get; set; }
	public int? Index { get; set; }
	public BsonDocument? Properties { get; set; }
}