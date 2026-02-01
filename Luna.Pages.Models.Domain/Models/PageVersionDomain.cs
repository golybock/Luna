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
		List<PageBlockSearchContent> blocks = ExtractBlocks(Document, pageDomain.Id.ToString());
		return new PageSearchDocument()
		{
			PageId = pageDomain.Id.ToString(),
			Title = pageDomain.Title,
			Description = pageDomain.Description,
			Content = ExtractDocumentText(Document),
			Blocks = blocks,
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

	private static List<PageBlockSearchContent> ExtractBlocks(object? document, string pageId)
	{
		JsonDocument? jsonDocument = TryGetJsonDocument(document);
		if (jsonDocument == null) return new List<PageBlockSearchContent>();

		List<PageBlockSearchContent> blocks = new List<PageBlockSearchContent>();
		HashSet<string> seen = new HashSet<string>();

		void Traverse(JsonElement node)
		{
			string? type = TryGetType(node);
			string? blockId = TryGetBlockId(node);
			string content = ExtractPlainText(node).Trim();

			if (!string.IsNullOrWhiteSpace(content) && !string.IsNullOrWhiteSpace(blockId))
			{
				if (ShouldIndexType(type) && seen.Add(blockId))
				{
					blocks.Add(new PageBlockSearchContent
					{
						BlockId = blockId,
						PageId = pageId,
						Type = type ?? "unknown",
						Content = content
					});
				}
			}

			if (node.ValueKind == JsonValueKind.Object &&
			    node.TryGetProperty("content", out JsonElement contentElement) &&
			    contentElement.ValueKind == JsonValueKind.Array)
			{
				foreach (JsonElement child in contentElement.EnumerateArray())
				{
					Traverse(child);
				}
			}
			else if (node.ValueKind == JsonValueKind.Array)
			{
				foreach (JsonElement child in node.EnumerateArray())
				{
					Traverse(child);
				}
			}
		}

		Traverse(jsonDocument.RootElement);

		return blocks;
	}

	private static string ExtractDocumentText(object? document)
	{
		JsonDocument? jsonDocument = TryGetJsonDocument(document);
		if (jsonDocument == null) return string.Empty;

		return ExtractPlainText(jsonDocument.RootElement).Trim();
	}

	private static JsonDocument? TryGetJsonDocument(object? document)
	{
		if (document == null) return null;

		if (document is JsonDocument jsonDocument) return jsonDocument;
		if (document is JsonElement jsonElement) return JsonDocument.Parse(jsonElement.GetRawText());

		try
		{
			string json = JsonSerializer.Serialize(document);
			return JsonDocument.Parse(json);
		}
		catch
		{
			return null;
		}
	}

	private static string ExtractPlainText(JsonElement node)
	{
		if (node.ValueKind == JsonValueKind.Object)
		{
			if (node.TryGetProperty("text", out JsonElement textElement) &&
			    textElement.ValueKind == JsonValueKind.String)
			{
				return textElement.GetString() ?? string.Empty;
			}

			if (node.TryGetProperty("content", out JsonElement contentElement) &&
			    contentElement.ValueKind == JsonValueKind.Array)
			{
				return string.Join(" ", contentElement.EnumerateArray().Select(ExtractPlainText));
			}
		}

		if (node.ValueKind == JsonValueKind.Array)
		{
			return string.Join(" ", node.EnumerateArray().Select(ExtractPlainText));
		}

		return string.Empty;
	}

	private static string? TryGetBlockId(JsonElement node)
	{
		if (node.TryGetProperty("attrs", out JsonElement attrsElement) &&
		    attrsElement.ValueKind == JsonValueKind.Object &&
		    attrsElement.TryGetProperty("blockId", out JsonElement idElement) &&
		    idElement.ValueKind == JsonValueKind.String)
		{
			return idElement.GetString();
		}

		return null;
	}

	private static string? TryGetType(JsonElement node)
	{
		if (node.TryGetProperty("type", out JsonElement typeElement) &&
		    typeElement.ValueKind == JsonValueKind.String)
		{
			return typeElement.GetString();
		}

		return null;
	}

	private static bool ShouldIndexType(string? type)
	{
		return type is "paragraph" or "heading" or "listItem" or "blockquote" or "codeBlock";
	}
}