using System.Text.Json;
using Luna.Pages.Models.Domain.Models;
using Luna.Users.Models.Domain.Models;
using StackExchange.Redis;

namespace Luna.Pages.Repositories.Repositories.Session;

public class SessionCacheRepository : ISessionCacheRepository
{
	private readonly IDatabase _redisDatabase;

	private static string PageUsersKey(string pageId, string userId) => $"page_users:{pageId}-{userId}";
	private static string PageCursorsKey(string pageId) => $"page_cursors:{pageId}";
	private static string ConnectionPageKey(string connectionId) => $"connection_page:{connectionId}";

	public SessionCacheRepository(string connectionString)
	{
		ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);
		_redisDatabase = redis.GetDatabase();
	}

	public async Task<string?> GetConnectionPageAsync(string connectionId)
	{
		RedisValue value = await _redisDatabase.StringGetAsync(ConnectionPageKey(connectionId));
		return value.HasValue ? value.ToString() : null;
	}

	public Task SetConnectionPageAsync(string connectionId, string pageId)
	{
		return _redisDatabase.StringSetAsync(ConnectionPageKey(connectionId), pageId);
	}

	public Task RemoveConnectionPageAsync(string connectionId)
	{
		return _redisDatabase.KeyDeleteAsync(ConnectionPageKey(connectionId));
	}


	public async Task AddUserToPageAsync(string pageId, string userId, UserDomain? userDomain)
	{
		UserDomain userData = userDomain ?? new UserDomain() {Id = Guid.Parse(userId)};

		await _redisDatabase.SetAddAsync(PageUsersKey(pageId, userId), JsonSerializer.Serialize(userData));
	}

	public async Task<IEnumerable<UserDomain>> GetPageUsersAsync(string pageId)
	{
		HashEntry[] entries = await _redisDatabase.HashGetAllAsync(PageUsersKey(pageId, "*"));

		List<UserDomain> users = new List<UserDomain>();

		foreach (HashEntry entry in entries)
		{
			if (entry.Value.IsNullOrEmpty) continue;

			UserDomain? user = JsonSerializer.Deserialize<UserDomain>(entry.Value!);

			if (user != null) users.Add(user);
		}

		return users;
	}

	public async Task<UserDomain?> GetPageUserByIdAsync(string pageId, string userId)
	{
		HashEntry[] user = await _redisDatabase.HashGetAllAsync(PageUsersKey(pageId, userId));

		return user.Length > 0 ? JsonSerializer.Deserialize<UserDomain>(user[0].ToString()) : null;
	}

	public async Task RemoveUserFromPageAsync(string pageId, string userId)
	{
		string key = PageUsersKey(pageId, userId);

		await _redisDatabase.SetRemoveAsync(key, userId);

		if (await _redisDatabase.SetLengthAsync(key) == 0)
		{
			await _redisDatabase.KeyDeleteAsync(key);
		}
	}

	public async Task<IEnumerable<UserCursorDomain>> GetPageCursorsAsync(string pageId)
	{
		HashEntry[] entries = await _redisDatabase.HashGetAllAsync(PageCursorsKey(pageId));

		List<UserCursorDomain> cursors = new List<UserCursorDomain>();

		foreach (HashEntry entry in entries)
		{
			if (entry.Value.IsNullOrEmpty) continue;

			UserCursorDomain? cursor = JsonSerializer.Deserialize<UserCursorDomain>(entry.Value!);

			if (cursor != null) cursors.Add(cursor);
		}

		return cursors;
	}

	public Task UpsertUserCursorAsync(string pageId, UserCursorDomain cursor)
	{
		string value = JsonSerializer.Serialize(cursor);

		return _redisDatabase.HashSetAsync(PageCursorsKey(pageId), cursor.UserId, value);
	}

	public async Task RemoveUserCursorAsync(string pageId, string userId)
	{
		string key = PageCursorsKey(pageId);

		await _redisDatabase.HashDeleteAsync(key, userId);

		if (await _redisDatabase.HashLengthAsync(key) == 0)
		{
			await _redisDatabase.KeyDeleteAsync(key);
		}
	}
}