using Luna.Users.Models.Database.Models;

namespace Luna.Users.Repositories.Repositories.Bookmark;

public interface IBookmarkRepository
{
	Task<IEnumerable<BookmarkDatabase>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 50);
	Task<BookmarkDatabase?> GetByIdAsync(Guid bookmarkId);
	Task<bool> CreateOrUpdateAsync(BookmarkDatabase bookmark);
	Task<bool> DeleteAsync(Guid bookmarkId);
	Task<bool> DeleteByUserIdAsync(Guid userId);
	Task<int> GetCountByUserIdAsync(Guid userId);
}