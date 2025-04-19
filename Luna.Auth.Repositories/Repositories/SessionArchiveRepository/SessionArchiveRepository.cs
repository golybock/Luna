using Luna.Auth.Models.Database.Models;
using Npgsql;
using Npgsql.Extension.Options;
using Npgsql.Extension.Repositories;

namespace Luna.Auth.Repositories.Repositories.SessionArchiveRepository;

public class SessionArchiveRepository : NpgsqlRepository, ISessionArchiveRepository
{
	private const string TableName = "session_archive";

	public SessionArchiveRepository(IDatabaseOptions databaseOptions) : base(databaseOptions) { }

	public async Task<bool> CreateSessionAsync(SessionDatabase session)
	{
		string query =
			$"insert into {TableName} (id, user_id, token, refresh_token, device, expires_at, user_agent, ip_address, revoked_at)" +
			$"values ($1, $2, $3, $4, $5, $6, $7, $8, $9)";

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = session.Id},
			new NpgsqlParameter() {Value = session.UserId},
			new NpgsqlParameter() {Value = session.Token},
			new NpgsqlParameter() {Value = session.RefreshToken},
			new NpgsqlParameter() {Value = session.Device == null ? DBNull.Value : session.Device},
			new NpgsqlParameter() {Value = session.ExpiresAt},
			new NpgsqlParameter() {Value = session.UserAgent == null ? DBNull.Value : session.UserAgent},
			new NpgsqlParameter() {Value = session.IpAddress == null ? DBNull.Value : session.IpAddress},
			new NpgsqlParameter() {Value = session.RevokedAt == null ? DBNull.Value : session.RevokedAt},
		];

		return await ExecuteAsync(query, parameters);
	}

	public async Task<SessionDatabase?> GetSessionAsync(Guid id)
	{
		string query = $"select * from {TableName} where id = $1";

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = id}
		];

		return await GetAsync<SessionDatabase>(query, parameters);
	}

	public async Task<IEnumerable<SessionDatabase>> GetSessionsByUserIdAsync(Guid userId)
	{
		string query = $"select * from {TableName} where user_id = $1";

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = userId}
		];

		return await GetListAsync<SessionDatabase>(query, parameters);
	}

	public async Task<bool> DeleteSessionAsync(Guid id)
	{
		string query = $"delete from {TableName} where id = $1";

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = id}
		];

		return await ExecuteAsync(query, parameters);
	}
}