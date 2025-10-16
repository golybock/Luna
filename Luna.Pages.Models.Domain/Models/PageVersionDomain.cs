using System.Text.Json;
using Luna.Pages.Models.Blank.Models;
using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.Database.Search;
using Luna.Pages.Models.View.Models;
using MongoDB.Bson;

namespace Luna.Pages.Models.Domain.Models;

public class PageVersionDomain
{
	public Guid Id { get; set; }
	public Guid PageId { get; set; }
	public int Version { get; set; }
	public IEnumerable<PageBlockDomain>? Content { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public Guid CreatedBy { get; set; }
	public string? ChangeDescription { get; set; }

	public static PageVersionDomain CreateInitial(Guid id, Guid pageId, Guid createdBy)
	{
		return new PageVersionDomain()
		{
			Id = id,
			PageId = pageId,
			Version = 1,
			Content = null,
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow,
			CreatedBy = createdBy,
			ChangeDescription = "Initial commit"
		};
	}

	public static PageVersionDomain FromDatabase(PageVersionDatabase pageVersionDatabase)
	{
		return new PageVersionDomain()
		{
			Id = Guid.Parse(pageVersionDatabase.Id),
			PageId = Guid.Parse(pageVersionDatabase.PageId),
			Version = pageVersionDatabase.Version,
			Content = pageVersionDatabase.Content?.Select(bsonValue => new PageBlockDomain()
			{
				Id = bsonValue["_id"].AsString,
				PageId = Guid.Parse(bsonValue["page_id"].AsString),
				Index = bsonValue["index"].AsInt32,
				Content = JsonDocument.Parse(bsonValue["content"]?.AsBsonDocument?.ToJson() ?? string.Empty),
				CreatedAt = bsonValue["created_at"].AsUniversalTime,
				UpdatedAt = bsonValue["updated_at"].AsUniversalTime,
				CreatedBy = Guid.Parse(bsonValue["created_by"].AsString),
				ParentId = bsonValue.AsBsonDocument.TryGetValue("parent_id", out BsonValue? parentId) && parentId.IsBsonDocument
					? Guid.Parse(parentId.AsString)
					: null,
				Properties = bsonValue.AsBsonDocument.TryGetValue("properties", out BsonValue? prop) && prop.IsBsonDocument
					? prop.AsBsonDocument
					: null,
				Type = bsonValue["type"].AsString,
				UpdatedBy = Guid.Parse(bsonValue["updated_by"].AsString),
			}),
			CreatedAt = pageVersionDatabase.CreatedAt,
			UpdatedAt = pageVersionDatabase.UpdatedAt,
			ChangeDescription = pageVersionDatabase.ChangeDescription,
			CreatedBy = Guid.Parse(pageVersionDatabase.CreatedBy),
		};
	}

	public static PageVersionDomain CreateFromBlank(Guid id, Guid pageId, Guid createdBy,
		UpdatePageContentBlank updatePageContentBlank)
	{
		return new PageVersionDomain()
		{
			Id = id,
			PageId = pageId,
			Version = 1,
			Content = updatePageContentBlank.Blocks.Select(item => PageBlockDomain.FromBlank(item, pageId, createdBy)),
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow,
			CreatedBy = createdBy,
			ChangeDescription = updatePageContentBlank.ChangeDescription
		};
	}

	public PageVersionDatabase ToDatabase()
	{
		return new PageVersionDatabase()
		{
			Id = Id.ToString(),
			PageId = PageId.ToString(),
			Version = Version,
			Content = new BsonArray((Content ?? []).Select(item => item.ToDatabase().ToBsonDocument()).ToList()),
			CreatedAt = CreatedAt,
			UpdatedAt = UpdatedAt,
			CreatedBy = CreatedBy.ToString(),
			ChangeDescription = ChangeDescription
		};
	}

	public PageVersionView ToView()
	{
		return new PageVersionView()
		{
			Id = Id,
			PageId = PageId,
			Version = Version,
			Content = Content?.Select(item => item.ToView()).ToList(),
			CreatedAt = CreatedAt,
			UpdatedAt = UpdatedAt,
			CreatedBy = CreatedBy,
			ChangeDescription = ChangeDescription
		};
	}

	public PageSearchDocument ToSearchDocument(PageDomain pageDomain)
	{
		return new PageSearchDocument()
		{
			PageId = pageDomain.Id.ToString(),
			Title = pageDomain.Title,
			Description = pageDomain.Description,
			Blocks = (Content ?? []).Select(item => item.ToSearchDocument()),
			UpdatedAt = UpdatedAt,
			WorkspaceId = pageDomain.WorkspaceId.ToString()
		};
	}
}