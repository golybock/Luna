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
	public object? Document { get; set; }
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
			Document = CreateEmptyDocument(),
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
			Document = ConvertFromBson(pageVersionDatabase.Document) ?? CreateEmptyDocument(),
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
			Document = updatePageContentBlank.Document ?? CreateEmptyDocument(),
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
			Document = ConvertToBsonDocument(Document),
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
			Document = Document,
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
			Blocks = [],
			UpdatedAt = UpdatedAt,
			WorkspaceId = pageDomain.WorkspaceId.ToString()
		};
	}

	private static object? ConvertFromBson(BsonDocument? document)
	{
		if (document == null) return null;
		return JsonDocument.Parse(document.ToJson());
	}

	private static BsonDocument? ConvertToBsonDocument(object? obj)
	{
		if (obj == null) return null;

		if (obj is BsonDocument bson) return bson;

		if (obj is JsonDocument jsonDoc)
		{
			return BsonDocument.Parse(jsonDoc.RootElement.GetRawText());
		}

		try
		{
			string json = JsonSerializer.Serialize(obj);
			return BsonDocument.Parse(json);
		}
		catch
		{
			return null;
		}
	}

	private static object CreateEmptyDocument()
	{
		return new { type = "doc", content = Array.Empty<object>() };
	}
}