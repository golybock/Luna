using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.Database.Models;
using Luna.Users.Models.Domain.Models;
using Luna.Users.Models.View.Models;
using Luna.Users.Repositories.Repositories.Bookmark;

namespace Luna.Users.Services.Services.Bookmark;

public class BookmarkService : IBookmarkService
{
	private readonly IBookmarkRepository _bookmarkRepository;

	public BookmarkService(IBookmarkRepository bookmarkRepository)
	{
		_bookmarkRepository = bookmarkRepository;
	}

	public async Task<IEnumerable<BookmarkView>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 50)
	{
		IEnumerable<BookmarkDatabase> bookmarks = await _bookmarkRepository.GetByUserIdAsync(userId, page, pageSize);

		return bookmarks
			.Select(BookmarkDomain.FromDatabase)
			.Select(bookmarkDomain => bookmarkDomain.ToView())
			.ToList();
	}

	public async Task<BookmarkView?> GetByIdAsync(Guid userId, Guid bookmarkId)
	{
		throw new NotImplementedException();
	}

	public async Task<BookmarkView?> GetByIdAsync(Guid bookmarkId)
	{
		BookmarkDatabase? bookmark = await _bookmarkRepository.GetByIdAsync(bookmarkId);

		return bookmark == null ? null : BookmarkDomain.FromDatabase(bookmark).ToView();
	}

	// Либо создание, либо изменение (только индекс)
	public async Task<bool> CreateOrUpdateAsync(Guid userId, Guid? bookmarkId, BookmarkBlank bookmark)
	{
		BookmarkDomain bookmarkDomain = BookmarkDomain.FromBlank(bookmark);

		// не указан id - создание новой закладки
		if (bookmarkId == null)
		{
			bookmarkDomain.Id = Guid.NewGuid();
			bookmarkDomain.UserId = userId;
			bookmarkDomain.Index = bookmark.Index ?? 1;
			bookmarkDomain.EntityType = 1; // TODO Get entity type from pages service
		}
		else
		{
			bookmarkDomain.Id = bookmarkId.Value;
		}

		return await _bookmarkRepository.CreateOrUpdateAsync(bookmarkDomain.ToDatabase());
	}

	public async Task<bool> DeleteAsync(Guid userId, Guid bookmarkId)
	{
		BookmarkDatabase? bookmark = await _bookmarkRepository.GetByIdAsync(bookmarkId);

		if (bookmark != null && bookmark.UserId == userId)
		{
			return await _bookmarkRepository.DeleteAsync(bookmarkId);
		}

		throw new Exception("Bookmark not found");
	}

	public async Task<bool> DeleteByUserIdAsync(Guid userId)
	{
		throw new NotImplementedException();
	}

	public async Task<int> GetCountByUserIdAsync(Guid userId)
	{
		throw new NotImplementedException();
	}
}