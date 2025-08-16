using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.Domain.Models;
using Luna.Users.Models.View.Models;

namespace Luna.Users.gRPC.Client.Services;

public interface IUserServiceClient
{
	Task<UserDomain?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
	Task<UserDomain?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
	Task<IEnumerable<UserDomain>> GetUsersByIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default);
	Task CreateUserAsync(Guid userId, UserBlank? userBlank = null, CancellationToken cancellationToken = default);
	Task UpdateUserAsync(Guid userId, UserBlank userBlank, CancellationToken cancellationToken = default);
	Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
	Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default);
	Task<bool> UserExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
	Task UpdateLastActiveAsync(Guid userId, CancellationToken cancellationToken = default);
}