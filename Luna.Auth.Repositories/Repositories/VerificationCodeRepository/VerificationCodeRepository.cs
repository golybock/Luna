using System.Text.Json;
using Luna.Auth.Models.Database.Models;
using StackExchange.Redis;

namespace Luna.Auth.Repositories.Repositories.VerificationCodeRepository;

public class VerificationCodeRepository : IVerificationCodeRepository
{
	private readonly IDatabase _redisDatabase;


	public VerificationCodeRepository(string connectionString)
	{
		ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);
		_redisDatabase = redis.GetDatabase();
	}

	public async Task CreateVerificationCodeAsync(string email, VerificationCodeDatabase verificationCode, TimeSpan expire)
	{
		await _redisDatabase.StringSetAsync(email, JsonSerializer.Serialize(verificationCode), expire);
	}

	public async Task<VerificationCodeDatabase?> GetVerificationCodeAsync(string email)
	{
		RedisValue data =  await _redisDatabase.StringGetAsync(email);

		if (!data.IsNullOrEmpty)
		{
			return JsonSerializer.Deserialize<VerificationCodeDatabase>(data!);
		}

		return null;
	}

	public async Task DeleteVerificationCodeAsync(string email)
	{
		await _redisDatabase.KeyDeleteAsync(email);
	}
}