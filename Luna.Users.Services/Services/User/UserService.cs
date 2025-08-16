using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.Database.Models;
using Luna.Users.Models.Domain.Models;
using Luna.Users.Models.Extensions.Extensions;
using Luna.Users.Models.View.Models;
using Luna.Users.Repositories.Repositories.User;
using Luna.Users.Services.Extensions;

namespace Luna.Users.Services.Services.User;

public class UserService : IUserService
{
	private readonly IUserRepository _userRepository;

	public UserService(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<UserView?> GetUserByIdAsync(Guid userId)
	{
		UserDatabase? user = await _userRepository.GetUserByIdAsync(userId);

		return user?.ToView();
	}

	public async Task<UserView?> GetUserByUsernameAsync(string username)
	{
		UserDatabase? user = await _userRepository.GetUserByUsernameAsync(username);

		return user?.ToView();
	}

	public async Task<IEnumerable<UserView>> GetUsersByIdsAsync(IEnumerable<Guid> userIds)
	{
		IEnumerable<UserDatabase> users = await _userRepository.GetUsersByIdsAsync(userIds);

		return users.Select(u => u.ToView()).ToList();
	}

	public async Task<bool> CreateUserAsync(Guid userId, UserBlank user)
	{
		if (string.IsNullOrEmpty(user.Username))
		{
			throw new Exception("Username is null");
		}

		UserDatabase? userDatabase = await _userRepository.GetUserByUsernameAsync(user.Username);

		if (userDatabase != null)
		{
			throw new Exception("User already exists");
		}

		UserDomain userDomain = user.ToDomain();
		userDomain.Id = userId;

		return await _userRepository.CreateUserAsync(userDomain.ToDatabase());
	}

	public async Task<bool> UpdateUserAsync(Guid userId, UserBlank user)
	{
		UserDatabase? userDatabase = await _userRepository.GetUserByIdAsync(userId);

		if (userDatabase == null)
		{
			throw new Exception("User not found");
		}

		UserDomain userDomain = user.ToDomain(userDatabase);

		return await _userRepository.UpdateUserAsync(userId, userDomain.ToDatabase());
	}

	public async Task<bool> DeleteUserAsync(Guid userId)
	{
		return await _userRepository.DeleteUserAsync(userId);
	}

	public async Task<bool> UserExistsAsync(Guid userId)
	{
		return await _userRepository.UserExistsAsync(userId);
	}

	public async Task<bool> UserExistsByUsernameAsync(string username)
	{
		return await _userRepository.UserExistsByUsernameAsync(username);
	}

	public async Task<bool> UpdateLastActiveAsync(Guid userId)
	{
		return await _userRepository.UpdateLastActiveAsync(userId);
	}
}