using Luna.Pages.Models.Database.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Luna.Pages.Models.Database.Models;

[BsonCollection("page_version")]
public class PageVersionDatabase
{
	[BsonId]
	[BsonRepresentation(BsonType.String)]
	public string Id { get; set; }

	[BsonElement("page_id")]
	[BsonRepresentation(BsonType.String)]
	public string PageId { get; set; }

	[BsonElement("version")]
	public int Version  { get; set; }

	[BsonElement("content")]
	public BsonDocument? Content { get; set; }

	[BsonElement("created_at")]
	[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
	public DateTime CreatedAt { get; set; }

	[BsonElement("updated_at")]
	[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
	public DateTime UpdatedAt { get; set; }

	[BsonElement("created_by")]
	[BsonRepresentation(BsonType.String)]
	public string CreatedBy { get; set; }

	[BsonElement("change_description")]
	public string? ChangeDescription { get; set; }
}