using Luna.Pages.Models.Blank.Models;
using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.View.Models;
using MongoDB.Bson;
using System.Text.Json;

namespace Luna.Pages.Models.Domain.Models;

public class PageBlockDomain
{
	public Guid Id { get; set; }
	public Guid PageId { get; set; }
	public string Type { get; set; } = null!;
	public object? Content { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public Guid CreatedBy { get; set; }
	public Guid UpdatedBy { get; set; }
	public Guid? ParentId { get; set; }
	public int Index { get; set; }
	public object? Properties { get; set; }

	public static PageBlockDomain FromDatabase(PageBlockDatabase block)
	{
		return new PageBlockDomain()
		{
			Id = block.Id,
			PageId = block.PageId,
			Type = block.Type,
			Content = block.Content,
			CreatedAt = block.CreatedAt,
			UpdatedAt = block.UpdatedAt,
			CreatedBy = block.CreatedBy,
			UpdatedBy = block.UpdatedBy,
			ParentId = block.ParentId,
			Index = block.Index,
			Properties = block.Properties
		};
	}

	public static PageBlockDomain FromBlank(PageBlockBlank blockBlank, Guid pageId, Guid createdBy)
	{
		return new PageBlockDomain()
		{
			Id = Guid.NewGuid(),
			Type = blockBlank.Type,
			PageId = pageId,
			Content = blockBlank.Content,
			ParentId = blockBlank.ParentId,
			CreatedBy = createdBy,
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow,
			UpdatedBy = createdBy,
			Index = blockBlank.Index,
			Properties = blockBlank.Properties
		};
	}

	public PageBlockView ToView()
	{
		return new PageBlockView()
		{
			Id = Id,
			Content = Content,
			Index = Index,
			ParentId = ParentId,
			Properties = Properties,
			Type = Type
		};
	}

	public PageBlockDatabase ToDatabase()
	{
		return new PageBlockDatabase()
		{
			Id = Id,
			Content = ConvertToBsonDocument(Content),
			Index = Index,
			ParentId = ParentId,
			Properties = ConvertToBsonDocument(Properties),
			Type = Type,
			PageId = PageId,
			CreatedAt = CreatedAt,
			UpdatedAt = UpdatedAt,
			CreatedBy = CreatedBy,
			UpdatedBy = UpdatedBy
		};
	}

	private static BsonDocument? ConvertToBsonDocument(object? obj)
	{
		if (obj == null)
			return null;

		// Если это уже BsonDocument, возвращаем как есть
		if (obj is BsonDocument bsonDoc)
		{
			// Проверяем на специальный маркер null от C#
			if (bsonDoc.Contains("_csharpnull") && bsonDoc["_csharpnull"].AsBoolean == true)
				return null;
			return bsonDoc;
		}

		// Если это JsonElement, конвертируем через JSON
		if (obj is JsonElement jsonElement)
		{
			// Проверяем, не является ли это null JsonElement
			if (jsonElement.ValueKind == JsonValueKind.Null)
				return null;

			var jsonString = jsonElement.GetRawText();
			var parsedDoc = BsonDocument.Parse(jsonString);

			// Проверяем на специальный маркер null от C#
			if (parsedDoc.Contains("_csharpnull") && parsedDoc["_csharpnull"].AsBoolean == true)
				return null;

			return parsedDoc;
		}

		// Если это другой объект, сериализуем в JSON, а затем парсим как BsonDocument
		try
		{
			var jsonString = JsonSerializer.Serialize(obj);
			var parsedDoc = BsonDocument.Parse(jsonString);

			// Проверяем на специальный маркер null от C#
			if (parsedDoc.Contains("_csharpnull") && parsedDoc["_csharpnull"].AsBoolean == true)
				return null;

			return parsedDoc;
		}
		catch
		{
			// В качестве fallback используем стандартное преобразование
			return obj.ToBsonDocument();
		}
	}
}