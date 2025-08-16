using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.Database.Models;
using Luna.Users.Models.Domain.Models;
using Luna.Users.Models.View.Models;

namespace Luna.Users.Models.Extensions.Extensions;

public static class BookmarkExtensions
{
	public static BookmarkView ToView(this BookmarkDatabase bookmark)
	{
		return new BookmarkView()
		{
			Id = bookmark.Id,
			EntityId = bookmark.EntityId,
			EntityType = bookmark.EntityType,
		};
	}

	public static BookmarkView ToView(this BookmarkDomain bookmark)
	{
		return new BookmarkView()
		{
			Id = bookmark.Id,
			EntityId = bookmark.EntityId,
			EntityType = bookmark.EntityType,
		};
	}

	public static BookmarkDomain ToDomain(this BookmarkDatabase bookmark)
	{
		return new BookmarkDomain()
		{
			Id = bookmark.Id,
			EntityId = bookmark.EntityId,
			EntityType = bookmark.EntityType,
			CreatedAt = bookmark.CreatedAt,
			UpdatedAt = bookmark.UpdatedAt,
			Index = bookmark.Index,
			UserId = bookmark.UserId,
			WorkspaceId = bookmark.WorkspaceId,
		};
	}

	public static BookmarkDomain ToDomain(this BookmarkBlank bookmark)
	{
		return new BookmarkDomain()
		{
			EntityId = bookmark.EntityId,
			WorkspaceId = bookmark.WorkspaceId,
			Index = bookmark.Index ?? 0 // на усмотрение сервиса
		};
	}

	public static BookmarkDatabase ToDatabase(this BookmarkDomain bookmark)
	{
		return new BookmarkDatabase()
		{
			Id = bookmark.Id,
			EntityId = bookmark.EntityId,
			EntityType = bookmark.EntityType,
			CreatedAt = bookmark.CreatedAt,
			UpdatedAt = bookmark.UpdatedAt,
			Index = bookmark.Index,
			UserId = bookmark.UserId,
			WorkspaceId = bookmark.WorkspaceId,
		};
	}
}