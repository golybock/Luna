using System.Net;
using System.Text.Json;
using Luna.Auth.Models.Database.Models;
using StackExchange.Redis;

namespace Luna.Auth.Repositories.Repositories.SessionRepository;

public class SessionRepository : ISessionRepository
{
	private readonly IDatabase _redisDatabase;
	private readonly IServer _server;

	private string Key(string userId, string sessionId) => $"{userId}:{sessionId}";
	private string Key(Guid userId, Guid sessionId) => $"{userId}:{sessionId}";

	public SessionRepository(string connectionString)
	{
		ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);
		_redisDatabase = redis.GetDatabase();

		// Получаем сервер из первого доступного эндпоинта
		EndPoint[] endpoints = redis.GetEndPoints();
		_server = redis.GetServer(endpoints.First());
	}

	public async Task<SessionDatabase?> GetSessionAsync(Guid userId, Guid sessionId)
	{
		string key = Key(userId, sessionId);

		string? session = await _redisDatabase.StringGetAsync(key);

		return session == null ? null : JsonSerializer.Deserialize<SessionDatabase>(session);
	}

	public async Task<IEnumerable<SessionDatabase>> GetUserSessionsAsync(Guid userId)
	{
		List<SessionDatabase> sessions = new List<SessionDatabase>();

		string keyPattern = Key(userId.ToString(), "*");

		foreach (RedisKey key in _server.Keys(pattern: keyPattern))
		{
			string? session = await _redisDatabase.StringGetAsync(key!);
			if (session != null) sessions.Add(JsonSerializer.Deserialize<SessionDatabase>(session)!);
		}

		return sessions;
	}

	public async Task<Boolean> SetSessionAsync(Guid userId, Guid sessionId, SessionDatabase session, TimeSpan ttl)
	{
		string key = Key(userId, sessionId);

		string value = JsonSerializer.Serialize(session);

		return await _redisDatabase.StringSetAsync(key, value, ttl);
	}

	public Task<Boolean> SessionExistsAsync(Guid? userId = null, Guid? sessionId = null)
	{
		string keyPattern = Key(userId.ToString() ?? "*", sessionId.ToString() ?? "*");

		return Task.FromResult(_server.Keys(pattern: keyPattern).Count() != 0);
	}

	public bool SessionExists(Guid? userId = null, Guid? sessionId = null)
	{
		string keyPattern = Key(userId.ToString() ?? "*", sessionId.ToString() ?? "*");

		return _server.Keys(pattern: keyPattern).Count() != 0;
	}

	public async Task<Boolean> UpdateSessionAsync(Guid userId, Guid sessionId, SessionDatabase session, TimeSpan ttl)
	{
		string key = Key(userId, sessionId);

		string value = JsonSerializer.Serialize(session);

		return await _redisDatabase.StringSetAsync(key, value, ttl);
	}

	public async Task<Int32> CloseSessionAsync(Guid? userId = null, Guid? sessionId = null)
	{
		string keyPattern = Key(userId.ToString() ?? "*", sessionId.ToString() ?? "*");

		// Получаем все ключи, соответствующие шаблону
		RedisKey[] keys = _server.Keys(pattern: keyPattern).ToArray();

		if (keys.Length == 0)
		{
			return 0; // Ничего не найдено для удаления
		}

		// Удаляем все найденные ключи
		foreach (RedisKey key in keys)
		{
			await _redisDatabase.KeyDeleteAsync(key);
		}

		return keys.Length;
	}
}