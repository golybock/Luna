using System.Net;
using System.Text.Json;
using Luna.Pages.Models.Domain.Models;
using StackExchange.Redis;

namespace Luna.Pages.Repositories.WorkspacePermissionRepository;

public class WorkspacePermissionCacheRepository : IWorkspacePermissionCacheRepository
{
	private readonly IDatabase _redisDatabase;
	private readonly IServer _server;

	private string Key(Guid workspaceId, Guid userId) => $"workspace_permission:{workspaceId}:{userId}";

	private readonly TimeSpan _defaultExpiration = TimeSpan.FromHours(1);

	public WorkspacePermissionCacheRepository(string connectionString)
	{
		ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);
		_redisDatabase = redis.GetDatabase();

		EndPoint[] endpoints = redis.GetEndPoints();
		_server = redis.GetServer(endpoints.First());
	}

	public async Task<WorkspaceUserPermission?> GetUserPermissionAsync(Guid workspaceId, Guid userId)
	{
		string key = Key(workspaceId, userId);

		string? result = await _redisDatabase.StringGetAsync(key);

		return result != null ? JsonSerializer.Deserialize<WorkspaceUserPermission>(result) : null;
	}

	public async Task<IEnumerable<WorkspaceUserPermission>> GetWorkspacePermissionsAsync(Guid workspaceId)
	{
		List<WorkspaceUserPermission> permissions = new List<WorkspaceUserPermission>();

		string pattern = $"workspace_permission:{workspaceId}:*";

		RedisKey[] keys = _server.Keys(pattern: pattern).ToArray();

		foreach (RedisKey key in keys)
		{
			string? permission = await _redisDatabase.StringGetAsync(key);
			if (permission != null) permissions.Add(JsonSerializer.Deserialize<WorkspaceUserPermission>(permission)!);
		}

		return permissions;
	}

	public async Task SetUserPermissionAsync(WorkspaceUserPermission permission)
	{
		string key = Key(permission.WorkspaceId, permission.UserId);

		string value = JsonSerializer.Serialize(permission);

		await _redisDatabase.StringSetAsync(key, value, _defaultExpiration);
	}

	public async Task DeleteUserPermissionAsync(Guid workspaceId, Guid userId)
	{
		string key = Key(workspaceId, userId);

		await _redisDatabase.KeyDeleteAsync(key);
	}

	public async Task DeleteUserPermissionByWorkspaceIdAsync(Guid workspaceId)
	{
		string pattern = $"workspace_permission:{workspaceId}:*";

		RedisKey[] keys = _server.Keys(pattern: pattern).ToArray();

		await _redisDatabase.KeyDeleteAsync(keys);
	}

	public async Task DeleteUserPermissionByUserIdAsync(Guid userId)
	{
		string pattern = $"workspace_permission:*:{userId}";

		RedisKey[] keys = _server.Keys(pattern: pattern).ToArray();

		await _redisDatabase.KeyDeleteAsync(keys);
	}
}