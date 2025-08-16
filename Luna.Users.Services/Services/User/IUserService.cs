using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.View.Models;

namespace Luna.Users.Services.Services.User;

public interface IUserService
{
	Task<UserView?> GetUserByIdAsync(Guid userId);
	Task<UserView?> GetUserByUsernameAsync(string username);
	Task<IEnumerable<UserView>> GetUsersByIdsAsync(IEnumerable<Guid> userIds);
	Task<bool> CreateUserAsync(Guid userId, UserBlank user);
	Task<bool> UpdateUserAsync(Guid userId, UserBlank user);
	Task<bool> DeleteUserAsync(Guid userId);
	Task<bool> UserExistsAsync(Guid userId);
	Task<bool> UserExistsByUsernameAsync(string username);
	Task<bool> UpdateLastActiveAsync(Guid userId);
}