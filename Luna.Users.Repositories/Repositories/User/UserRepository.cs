using System.Text.Json;
using Luna.Users.Models.Database.Models;
using Luna.Users.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Luna.Users.Repositories.Repositories.User;

public class UserRepository : IUserRepository
{
	private readonly LunaUsersContext _context;

	public UserRepository(LunaUsersContext context)
	{
		_context = context;
	}

	public async Task<UserDatabase?> GetUserByIdAsync(Guid userId)
	{
		return await _context.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(user => user.Id == userId);
	}

	public async Task<UserDatabase?> GetUserByUsernameAsync(string username)
	{
		UserDatabase? user =  await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
		return user;
	}

	public async Task<IEnumerable<UserDatabase>> GetUsersByIdsAsync(IEnumerable<Guid> userIds)
	{
		List<Guid> userIdsList = userIds.ToList();

		if (!userIdsList.Any())
			return [];

		return await _context.Users
			.AsNoTracking()
			.Where(user => userIdsList.Contains(user.Id))
			.ToListAsync();
	}

	public async Task<bool> CreateUserAsync(UserDatabase user)
	{
		if (user.Id == Guid.Empty)
		{
			throw new Exception("User id is empty");
		}

		user.CreatedAt = DateTime.UtcNow;
		user.UpdatedAt = DateTime.UtcNow;
		user.LastActive = DateTime.UtcNow;

		_context.Users.Add(user);
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> UpdateUserAsync(Guid userId, UserDatabase user)
	{
		UserDatabase? existingUser = await GetUserByIdAsync(userId);

		if (existingUser == null)
			throw new InvalidOperationException($"User with ID {user.Id} not found");

		// Обновляем только изменяемые поля
		existingUser.Username = user.Username;
		existingUser.DisplayName = user.DisplayName;
		existingUser.Image = user.Image;
		existingUser.Bio = user.Bio;
		existingUser.UpdatedAt = DateTime.UtcNow;
		existingUser.LastActive = DateTime.UtcNow;

		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> DeleteUserAsync(Guid userId)
	{
		UserDatabase? user = await GetUserByIdAsync(userId);

		if (user == null)
			return false;

		_context.Users.Remove(user);
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> UserExistsAsync(Guid userId)
	{
		return await _context.Users
			.AsNoTracking()
			.AnyAsync(u => u.Id == userId);
	}

	public async Task<bool> UserExistsByUsernameAsync(string username)
	{
		return await _context.Users
			.AsNoTracking()
			.AnyAsync(u => u.Username == username);
	}

	public async Task<bool> UpdateLastActiveAsync(Guid userId)
	{
		await _context.Users
			.Where(u => u.Id == userId)
			.ExecuteUpdateAsync(s => s.SetProperty(u => u.LastActive, DateTime.UtcNow));

		return await _context.SaveChangesAsync() > 0;
	}
}