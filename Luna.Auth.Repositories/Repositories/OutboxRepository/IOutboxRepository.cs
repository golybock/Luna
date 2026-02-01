using Luna.Tools.SharedModels.Models.Outbox;

namespace Luna.Auth.Repositories.Repositories.OutboxRepository;

public interface IOutboxRepository
{
	Task<bool> AddMessageAsync(OutboxMessageDatabase message);

	Task<IReadOnlyList<OutboxMessageDatabase>> AcquirePendingMessagesAsync(int batchSize, DateTime lockedUntil);

	Task<bool> MarkProcessedAsync(Guid id, DateTime processedAt);

	Task<bool> MarkForRetryAsync(Guid id, string error, DateTime lockedUntil);

	Task<bool> MarkFailedAsync(Guid id, string error);
}
