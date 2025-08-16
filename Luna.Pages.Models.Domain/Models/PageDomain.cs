using System.Text.Json;
using Luna.Pages.Models.Blank.Models;
using Luna.Pages.Models.Database.Models;
using Luna.Pages.Models.View.Additional;
using Luna.Pages.Models.View.Models;
using MongoDB.Bson;

namespace Luna.Pages.Models.Domain.Models;

public class PageDomain
{
	public Guid Id { get; set; }
	public string Title { get; set; } = null!;
	public string? Description { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public Guid WorkspaceId { get; set; }
	public DateTime? DeletedAt { get; set; }
	public int LatestVersion { get; set; }
	public Guid OwnerId { get; set; }
	public Guid? ParentId { get; set; }
	public string? Icon { get; set; }
	public string? Cover { get; set; }
	public string? Emoji { get; set; }
	public string Type { get; set; } = null!;
	public string? Path { get; set; }
	public int? Index { get; set; }
	public bool IsTemplate { get; set; }
	public DateTime? ArchivedAt { get; set; }
	public bool Pinned { get; set; }
	public string? CustomSlug { get; set; }
	public object? Properties { get; set; }

	public List<PageDomain> ChildPages { get; set; } = new List<PageDomain>();

	public static PageDomain FromBlank(Guid id, Guid ownerId, CreatePageBlank createPageBlank)
	{
		return new PageDomain()
		{
			Id = id,
			OwnerId = ownerId,
			Title = createPageBlank.Title,
			WorkspaceId = createPageBlank.WorkspaceId,
			Description = "",
			LatestVersion = 1,
			ParentId = createPageBlank.ParentId,
			Emoji = createPageBlank.Emoji,
			Icon = createPageBlank.Icon,
			CreatedAt = DateTime.UtcNow,
			Type = "Page",
		};
	}

	public static PageDomain FromDatabase(PageDatabase page)
	{
		return new PageDomain()
		{
			Id = Guid.Parse(page.Id),
			OwnerId = Guid.Parse(page.OwnerId),
			Title = page.Title,
			WorkspaceId = Guid.Parse(page.WorkspaceId),
			ParentId = page.ParentId != null ? Guid.Parse(page.ParentId) : null,
			Emoji = page.Emoji,
			LatestVersion = page.LatestVersion,
			Icon = page.Icon,
			CreatedAt = page.CreatedAt,
			Type = page.Type,
			ArchivedAt = page.ArchivedAt,
			Description = page.Description,
			IsTemplate = page.IsTemplate,
			Cover = page.Cover,
			CustomSlug = page.CustomSlug,
			DeletedAt = page.DeletedAt,
			Index = page.Index,
			UpdatedAt = page.UpdatedAt,
			Properties = page.Properties,
			Pinned = page.Pinned,
			Path = page.Path,
		};
	}

	public static PageDomain FromDatabase(PageDatabase page, IEnumerable<PageDatabase> childPages)
	{
		return new PageDomain()
		{
			Id = Guid.Parse(page.Id),
			OwnerId = Guid.Parse(page.OwnerId),
			Title = page.Title,
			WorkspaceId = Guid.Parse(page.WorkspaceId),
			ParentId = page.ParentId != null ? Guid.Parse(page.ParentId) : null,
			Emoji = page.Emoji,
			Icon = page.Icon,
			CreatedAt = page.CreatedAt,
			Type = page.Type,
			ChildPages = childPages.Select(FromDatabase).ToList()
		};
	}

	public LightPageView ToLightPageView()
	{
		return new LightPageView()
		{
			Id = Id,
			Title = Title,
			Emoji = Emoji,
			ChildPages = ChildPages.Select(item => item.ToLightPageView()).ToList()
		};
	}

	public PageView ToView()
	{
		return new PageView()
		{
			Id = Id,
			Title = Title,
			Emoji = Emoji,
			Index = Index,
			ParentId = ParentId,
			CreatedAt = CreatedAt,
			Type = Type,
			Properties = Properties,
			WorkspaceId = WorkspaceId,
			ArchivedAt = ArchivedAt,
			Pinned = Pinned,
			CustomSlug = CustomSlug,
			Cover = Cover,
			Description = Description,
			Icon = Icon,
			IsTemplate = IsTemplate,
			Path = Path,
			LatestVersion = LatestVersion,
			OwnerId = OwnerId,
			UpdatedAt = UpdatedAt
		};
	}

	public PageDatabase ToDatabase()
	{
		return new PageDatabase()
		{
			Id = this.Id.ToString(),
			Title = this.Title,
			Description = this.Description,
			CreatedAt = this.CreatedAt,
			UpdatedAt = this.UpdatedAt,
			WorkspaceId = this.WorkspaceId.ToString(),
			DeletedAt = this.DeletedAt,
			LatestVersion = this.LatestVersion,
			OwnerId = this.OwnerId.ToString(),
			ParentId = this.ParentId?.ToString(),
			Icon = this.Icon,
			Cover = this.Cover,
			Emoji = this.Emoji,
			Type = this.Type,
			Path = this.Path,
			Index = this.Index,
			IsTemplate = this.IsTemplate,
			ArchivedAt = this.ArchivedAt,
			Pinned = this.Pinned,
			CustomSlug = this.CustomSlug,
			Properties = this.Properties.ToBsonDocument(),
		};
	}
}