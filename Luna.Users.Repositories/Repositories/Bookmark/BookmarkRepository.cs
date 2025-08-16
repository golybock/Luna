using Luna.Users.Models.Database.Models;
using Luna.Users.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Luna.Users.Repositories.Repositories.Bookmark;

public class BookmarkRepository : IBookmarkRepository
{
	private readonly LunaUsersContext _context;

	public BookmarkRepository(LunaUsersContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<BookmarkDatabase>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 50)
	{
		return await _context.Bookmarks
			.AsNoTracking()
			.Where(b => b.UserId == userId)
			.OrderByDescending(b => b.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();
	}

	public async Task<BookmarkDatabase?> GetByIdAsync(Guid bookmarkId)
	{
		return await _context.Bookmarks
			.AsNoTracking()
			.FirstOrDefaultAsync(b => b.Id == bookmarkId);
	}

	public async Task<bool> CreateOrUpdateAsync(BookmarkDatabase bookmark)
	{
		BookmarkDatabase? existing = await _context.Bookmarks
			.FirstOrDefaultAsync(b => b.Id == bookmark.Id);

		if (existing == null)
		{
			bookmark.Id = Guid.NewGuid();
			bookmark.CreatedAt = DateTime.UtcNow;
			bookmark.UpdatedAt = DateTime.UtcNow;

			_context.Bookmarks.Add(bookmark);
		}
		else
		{
			existing.Index = bookmark.Index;
			existing.UpdatedAt = DateTime.UtcNow;
		}

		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> DeleteAsync(Guid bookmarkId)
	{
		int deleted = await _context.Bookmarks
			.Where(b => b.Id == bookmarkId)
			.ExecuteDeleteAsync();

		return deleted > 0;
	}

	public async Task<bool> DeleteByUserIdAsync(Guid userId)
	{
		int deleted = await _context.Bookmarks
			.Where(b => b.UserId == userId)
			.ExecuteDeleteAsync();

		return deleted > 0;
	}

	public async Task<int> GetCountByUserIdAsync(Guid userId)
	{
		return await _context.Bookmarks
			.AsNoTracking()
			.CountAsync(b => b.UserId == userId);
	}
}