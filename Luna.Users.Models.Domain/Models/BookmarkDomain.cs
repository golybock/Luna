using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.Database.Models;
using Luna.Users.Models.View.Models;

namespace Luna.Users.Models.Domain.Models;

public class BookmarkDomain
{
	public Guid Id { get; set; }

	public Guid UserId { get; set; }

	public Guid EntityId { get; set; }

	public int EntityType { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }

	public Guid WorkspaceId { get; set; }

	public int Index { get; set; }

	public static BookmarkDomain FromBlank(BookmarkBlank bookmarkBlank)
	{
		return new BookmarkDomain()
		{
			EntityId = bookmarkBlank.EntityId,
			WorkspaceId = bookmarkBlank.WorkspaceId,
			Index = bookmarkBlank.Index ?? 0 // на усмотрение сервиса
		};
	}

	public static BookmarkDomain FromDatabase(BookmarkDatabase bookmarkDatabase)
	{
		return new BookmarkDomain()
		{
			Id = bookmarkDatabase.Id,
			EntityId = bookmarkDatabase.EntityId,
			EntityType = bookmarkDatabase.EntityType,
			CreatedAt = bookmarkDatabase.CreatedAt,
			UpdatedAt = bookmarkDatabase.UpdatedAt,
			Index = bookmarkDatabase.Index,
			UserId = bookmarkDatabase.UserId,
			WorkspaceId = bookmarkDatabase.WorkspaceId,
		};
	}

	public BookmarkView ToView()
	{
		return new BookmarkView()
		{
			Id = Id,
			EntityId = EntityId,
			EntityType = EntityType,
		};
	}

	public BookmarkDatabase ToDatabase()
	{
		return new BookmarkDatabase()
		{
			Id = Id,
			EntityId = EntityId,
			EntityType = EntityType,
			CreatedAt = CreatedAt,
			UpdatedAt = UpdatedAt,
			Index = Index,
			UserId = UserId,
			WorkspaceId = WorkspaceId,
		};
	}
}