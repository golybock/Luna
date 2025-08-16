using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.View.Models;

namespace Luna.Users.Services.Services.Bookmark;

public interface IBookmarkService
{
	Task<IEnumerable<BookmarkView>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 50);
	Task<BookmarkView?> GetByIdAsync(Guid userId, Guid bookmarkId);
	Task<bool> CreateOrUpdateAsync(Guid userId, Guid? bookmarkId, BookmarkBlank bookmark);
	Task<bool> DeleteAsync(Guid userId, Guid bookmarkId);
	Task<bool> DeleteByUserIdAsync(Guid userId);
	Task<int> GetCountByUserIdAsync(Guid userId);
}