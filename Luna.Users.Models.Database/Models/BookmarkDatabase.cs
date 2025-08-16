namespace Luna.Users.Models.Database.Models;

public class BookmarkDatabase
{
	public Guid Id { get; set; }

	public Guid UserId { get; set; }

	public Guid EntityId { get; set; }

	public int EntityType { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }

	public Guid WorkspaceId { get; set; }

	public int Index { get; set; }
}