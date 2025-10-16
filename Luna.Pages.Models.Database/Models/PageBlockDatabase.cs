using Luna.Pages.Models.Database.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Luna.Pages.Models.Database.Models;

[BsonCollection("page_block")]
public class PageBlockDatabase
{
	[BsonId]
	[BsonElement("_id")]
	[BsonRepresentation(BsonType.String)]
	public string Id { get; set; }

	[BsonElement("page_id")]
	[BsonRepresentation(BsonType.String)]
	public string PageId { get; set; }

	[BsonElement("type")]
	public string Type { get; set; } = null!;

	[BsonElement("content")]
	[BsonIgnoreIfNull]
	public BsonValue? Content { get; set; }

	[BsonElement("created_at")]
	[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
	public DateTime CreatedAt { get; set; }

	[BsonElement("updated_at")]
	[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
	public DateTime UpdatedAt { get; set; }

	[BsonElement("created_by")]
	[BsonRepresentation(BsonType.String)]
	public string CreatedBy { get; set; }

	[BsonElement("updated_by")]
	[BsonRepresentation(BsonType.String)]
	public Guid UpdatedBy { get; set; }

	[BsonElement("parent_id")]
	[BsonRepresentation(BsonType.String)]
	public string? ParentId { get; set; }

	[BsonElement("index")]
	public int Index { get; set; }

	[BsonElement("properties")]
	[BsonIgnoreIfNull]
	public BsonValue? Properties { get; set; }
}