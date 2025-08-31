using System.Text.Json;
using Luna.Pages.Models.Blank.Models;
using Luna.Pages.Models.Database.Models;
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

	private static object? ExtractBsonValue(BsonDocument document, string fieldName)
	{
		if (!document.Contains(fieldName))
			return null;

		var bsonValue = document[fieldName];

		// Проверяем на null
		if (bsonValue.IsBsonNull)
			return null;

		// Проверяем на специальный маркер C# null
		if (bsonValue.IsBsonDocument)
		{
			var bsonDoc = bsonValue.AsBsonDocument;
			if (bsonDoc.Contains("_csharpnull") && bsonDoc["_csharpnull"].AsBoolean == true)
				return null;
			return bsonDoc;
		}

		return bsonValue;
	}

	public static PageVersionDomain? FromDatabase(PageVersionDatabase pageVersionDatabase)
	{
		return new PageVersionDomain()
		{
			Id = Guid.Parse(pageVersionDatabase.Id),
			PageId = Guid.Parse(pageVersionDatabase.PageId),
			Version = pageVersionDatabase.Version,
			// Исправление: преобразуем BsonArray напрямую в PageBlockDomain
			Content = pageVersionDatabase.Content?.Select(bsonValue => new PageBlockDomain(){}),
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
			Content =
				Content?.Select(item => item.ToDatabase()).ToBsonDocument(),
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
}