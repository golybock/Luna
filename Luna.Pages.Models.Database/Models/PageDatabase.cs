using Luna.Pages.Models.Database.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Luna.Pages.Models.Database.Models;

[BsonCollection("page")]
public class PageDatabase
{
    [BsonId]
    public string Id { get; set; } = null!;

    [BsonElement("title")]
    public string Title { get; set; } = null!;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("created_at")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("workspace_id")]
    public string WorkspaceId { get; set; }

    [BsonElement("deleted_at")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DeletedAt { get; set; }

    [BsonElement("latest_version")]
    public int LatestVersion { get; set; }

    [BsonElement("owner_id")]
    public string OwnerId { get; set; }

    [BsonElement("parent_id")]
    public string? ParentId { get; set; }

    [BsonElement("icon")]
    public string? Icon { get; set; }

    [BsonElement("cover")]
    public string? Cover { get; set; }

    [BsonElement("emoji")]
    public string? Emoji { get; set; }

    [BsonElement("type")]
    public string Type { get; set; } = null!;

    [BsonElement("path")]
    public string? Path { get; set; }

    [BsonElement("index")]
    public int? Index { get; set; }

    [BsonElement("is_template")]
    public bool IsTemplate { get; set; }

    [BsonElement("archived_at")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? ArchivedAt { get; set; }

    [BsonElement("pinned")]
    public bool Pinned { get; set; }

    [BsonElement("custom_slug")]
    public string? CustomSlug { get; set; }

    [BsonElement("properties")]
    public BsonDocument? Properties { get; set; }
}