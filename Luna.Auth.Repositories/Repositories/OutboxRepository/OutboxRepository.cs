using Luna.Tools.Database.Npgsql.Options;
using Luna.Tools.Database.Npgsql.Repositories;
using Luna.Tools.SharedModels.Models.Outbox;
using Npgsql;

namespace Luna.Auth.Repositories.Repositories.OutboxRepository;

public class OutboxRepository : NpgsqlRepository, IOutboxRepository
{
	public OutboxRepository(IDatabaseOptions databaseOptions) : base(databaseOptions) { }

	public async Task<bool> AddMessageAsync(OutboxMessageDatabase message)
	{
		string query = SqlQueries.GetSql(SqlQueries.OutboxRepository.Create);

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = message.Id},
			new NpgsqlParameter() {Value = message.Type},
			new NpgsqlParameter() {Value = message.Payload},
			new NpgsqlParameter() {Value = message.Status},
			new NpgsqlParameter() {Value = message.CreatedAt},
			new NpgsqlParameter() {Value = message.Attempts}
		];

		return await ExecuteAsync(query, parameters);
	}

	public async Task<IReadOnlyList<OutboxMessageDatabase>> AcquirePendingMessagesAsync(int batchSize, DateTime lockedUntil)
	{
		string query = SqlQueries.GetSql(SqlQueries.OutboxRepository.AcquirePending);

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = OutboxMessageStatus.Processing},
			new NpgsqlParameter() {Value = lockedUntil},
			new NpgsqlParameter() {Value = OutboxMessageStatus.Pending},
			new NpgsqlParameter() {Value = batchSize}
		];

		IEnumerable<OutboxMessageDatabase> result = await GetListAsync<OutboxMessageDatabase>(query, parameters);
		return result.ToList();
	}

	public async Task<bool> MarkProcessedAsync(Guid id, DateTime processedAt)
	{
		string query = SqlQueries.GetSql(SqlQueries.OutboxRepository.MarkProcessed);

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = OutboxMessageStatus.Sent},
			new NpgsqlParameter() {Value = processedAt},
			new NpgsqlParameter() {Value = id}
		];

		return await ExecuteAsync(query, parameters);
	}

	public async Task<bool> MarkForRetryAsync(Guid id, string error, DateTime lockedUntil)
	{
		string query = SqlQueries.GetSql(SqlQueries.OutboxRepository.MarkForRetry);

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = OutboxMessageStatus.Pending},
			new NpgsqlParameter() {Value = error},
			new NpgsqlParameter() {Value = lockedUntil},
			new NpgsqlParameter() {Value = id}
		];

		return await ExecuteAsync(query, parameters);
	}

	public async Task<bool> MarkFailedAsync(Guid id, string error)
	{
		string query = SqlQueries.GetSql(SqlQueries.OutboxRepository.MarkFailed);

		NpgsqlParameter[] parameters =
		[
			new NpgsqlParameter() {Value = OutboxMessageStatus.Failed},
			new NpgsqlParameter() {Value = error},
			new NpgsqlParameter() {Value = id}
		];

		return await ExecuteAsync(query, parameters);
	}
}
