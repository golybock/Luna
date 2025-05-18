using Luna.Auth.Models.Database.Models;
using Luna.Tools.Database.Npgsql.Options;
using Luna.Tools.Database.Npgsql.Repositories;
using Npgsql;

namespace Luna.Auth.Repositories.Repositories.SessionArchiveRepository;

public class SessionArchiveRepository : NpgsqlRepository, ISessionArchiveRepository
{
	public SessionArchiveRepository(IDatabaseOptions databaseOptions) : base(databaseOptions) { }

	public async Task<bool> CreateSessionAsync(SessionDatabase session)
	{
		string query = SqlQueries.GetSql(SqlQueries.SessionArchiveRepository.Create);

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

	public async Task<bool> CreateSessionAsync(SessionDatabase session, NpgsqlTransaction transaction)
	{
		string query = SqlQueries.GetSql(SqlQueries.SessionArchiveRepository.Create);

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

		await using NpgsqlCommand cmd = new NpgsqlCommand(query, transaction.Connection, transaction);
		cmd.Parameters.AddRange(parameters);
		int rowsAffected = await cmd.ExecuteNonQueryAsync();
		return rowsAffected > 0;
	}

	public async Task<int> CreateSessionsAsync(IEnumerable<SessionDatabase> sessions)
	{
		int createdSessions = 0;

		try
		{
			await ExecuteTransactionAsync(async transaction =>
			{
				foreach (SessionDatabase session in sessions)
				{
					bool created = await CreateSessionAsync(session, transaction);
					if (created) createdSessions++;
				}
			});

			return createdSessions;
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public async Task<SessionDatabase?> GetSessionAsync(Guid id)
	{
		string query = SqlQueries.GetSql(SqlQueries.SessionArchiveRepository.GetById);

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = id}
		];

		return await GetAsync<SessionDatabase>(query, parameters);
	}

	public async Task<IEnumerable<SessionDatabase>> GetSessionsByUserIdAsync(Guid userId)
	{
		string query = SqlQueries.GetSql(SqlQueries.SessionArchiveRepository.GetByUserId);

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = userId}
		];

		return await GetListAsync<SessionDatabase>(query, parameters);
	}

	public async Task<bool> DeleteSessionAsync(Guid id)
	{
		string query = SqlQueries.GetSql(SqlQueries.SessionArchiveRepository.DeleteSession);

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = id}
		];

		return await ExecuteAsync(query, parameters);
	}

	public async Task<bool> DeleteSessionsAsync(IEnumerable<Guid> ids)
	{
		string query = SqlQueries.GetSql(SqlQueries.SessionArchiveRepository.DeleteSessions);

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = ids}
		];

		return await ExecuteAsync(query, parameters);
	}
}