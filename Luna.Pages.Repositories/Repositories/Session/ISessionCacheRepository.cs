using Luna.Pages.Models.Domain.Models;
using Luna.Users.Models.Domain.Models;

namespace Luna.Pages.Repositories.Repositories.Session;

public interface ISessionCacheRepository
{
	Task<string?> GetConnectionPageAsync(string connectionId);
	Task SetConnectionPageAsync(string connectionId, string pageId);
	Task RemoveConnectionPageAsync(string connectionId);
	Task AddUserToPageAsync(string pageId, string userId, UserDomain? userDomain);
	Task<IEnumerable<UserDomain>> GetPageUsersAsync(string pageId);
	Task<UserDomain?> GetPageUserByIdAsync(string pageId, string userId);
	Task RemoveUserFromPageAsync(string pageId, string userId);
	Task<IEnumerable<UserCursorDomain>> GetPageCursorsAsync(string pageId);
	Task UpsertUserCursorAsync(string pageId, UserCursorDomain cursor);
	Task RemoveUserCursorAsync(string pageId, string userId);
}