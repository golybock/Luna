using Luna.Users.Models.Database.Models;

namespace Luna.Users.Repositories.Repositories.User;

public interface IUserRepository
{
	Task<UserDatabase?> GetUserByIdAsync(Guid userId);
	Task<UserDatabase?> GetUserByUsernameAsync(string username);
	Task<IEnumerable<UserDatabase>> GetUsersByIdsAsync(IEnumerable<Guid> userIds);
	Task<bool> CreateUserAsync(UserDatabase user);
	Task<bool> UpdateUserAsync(Guid userId, UserDatabase user);
	Task<bool> DeleteUserAsync(Guid userId);
	Task<bool> UserExistsAsync(Guid userId);
	Task<bool> UserExistsByUsernameAsync(string username);
	Task<bool> UpdateLastActiveAsync(Guid userId);
}