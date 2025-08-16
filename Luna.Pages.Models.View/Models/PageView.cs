namespace Luna.Pages.Models.View.Models;

public class PageView
{
	public Guid Id { get; set; }
	public string Title { get; set; } = null!;
	public string? Description { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public Guid WorkspaceId { get; set; }
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
}