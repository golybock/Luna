using Luna.Pages.Models.Database.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Luna.Pages.Models.Database.Models;

[BsonCollection("page_comment")]
public class PageCommentDatabase
{
	[BsonId]
	[BsonRepresentation(BsonType.String)]
	public Guid Id { get; set; }

	[BsonElement("page_id")]
	[BsonRepresentation(BsonType.String)]
	public Guid PageId { get; set; }

	[BsonElement("user_id")]
	[BsonRepresentation(BsonType.String)]
	public Guid UserId { get; set; }

	[BsonElement("content")]
	public string? Content { get; set; }

	[BsonElement("deleted_at")]
	[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
	public DateTime? DeletedAt { get; set; }

	[BsonElement("created_at")]
	[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
	public DateTime CreatedAt { get; set; }

	[BsonElement("updated_at")]
	[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
	public DateTime? UpdatedAt { get; set; }

	[BsonElement("parent_id")]
	[BsonRepresentation(BsonType.String)]
	public Guid? ParentId { get; set; }

	[BsonElement("block_id")]
	[BsonRepresentation(BsonType.String)]
	public Guid? BlockId { get; set; }

	[BsonElement("reactions")]
	public BsonDocument? Reactions { get; set; }
}