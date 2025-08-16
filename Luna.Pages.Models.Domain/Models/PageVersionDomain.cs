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
	public int Version  { get; set; }
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

	public static PageVersionDomain? FromDatabase(PageVersionDatabase pageVersionDatabase)
	{
		return new PageVersionDomain()
		{
			Id = Guid.Parse(pageVersionDatabase.Id),
			PageId = Guid.Parse(pageVersionDatabase.PageId),
			Version = pageVersionDatabase.Version,
			Content = JsonSerializer.Deserialize<IEnumerable<PageBlockDomain>>(pageVersionDatabase.Content.ToJson()),
			CreatedAt = pageVersionDatabase.CreatedAt,
			UpdatedAt = pageVersionDatabase.UpdatedAt,
			ChangeDescription = pageVersionDatabase.ChangeDescription,
			CreatedBy = Guid.Parse(pageVersionDatabase.CreatedBy),
		};
	}

	public static PageVersionDomain CreateFromBlank(Guid id, Guid pageId, Guid createdBy, UpdatePageContentBlank updatePageContentBlank)
	{
		return new PageVersionDomain()
		{
			Id = id,
			PageId = pageId,
			Version = 1,
			Content = JsonSerializer.Deserialize<IEnumerable<PageBlockDomain>>(updatePageContentBlank.ToJson()),
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
			Content = Content.ToBsonDocument(),
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