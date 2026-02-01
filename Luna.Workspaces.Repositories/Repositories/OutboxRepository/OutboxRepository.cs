using Luna.Tools.SharedModels.Models.Outbox;
using Luna.Workspaces.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Luna.Workspaces.Repositories.Repositories.OutboxRepository;

public class OutboxRepository : IOutboxRepository
{
	private readonly LunaWorkspacesContext _context;

	public OutboxRepository(LunaWorkspacesContext context)
	{
		_context = context;
	}

	public async Task<bool> AddMessageAsync(OutboxMessageDatabase message)
	{
		await _context.OutboxMessages.AddAsync(message);
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<IReadOnlyList<OutboxMessageDatabase>> AcquirePendingMessagesAsync(int batchSize, DateTime lockedUntil)
	{
		FormattableString sql = $@"
update outbox_messages
set status = {OutboxMessageStatus.Processing},
    locked_until = {lockedUntil},
    attempts = attempts + 1
where id in (
    select id
    from outbox_messages
    where status = {OutboxMessageStatus.Pending}
      and (locked_until is null or locked_until < now())
    order by created_at
    limit {batchSize}
    for update skip locked
)
returning *;";

		return await _context.OutboxMessages
			.FromSqlInterpolated(sql)
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<bool> MarkProcessedAsync(Guid id, DateTime processedAt)
	{
		OutboxMessageDatabase? message = await _context.OutboxMessages.FirstOrDefaultAsync(item => item.Id == id);
		if (message == null) return false;

		message.Status = OutboxMessageStatus.Sent;
		message.ProcessedAt = processedAt;
		message.LockedUntil = null;
		message.LastError = null;

		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> MarkForRetryAsync(Guid id, string error, DateTime lockedUntil)
	{
		OutboxMessageDatabase? message = await _context.OutboxMessages.FirstOrDefaultAsync(item => item.Id == id);
		if (message == null) return false;

		message.Status = OutboxMessageStatus.Pending;
		message.LastError = error;
		message.LockedUntil = lockedUntil;

		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> MarkFailedAsync(Guid id, string error)
	{
		OutboxMessageDatabase? message = await _context.OutboxMessages.FirstOrDefaultAsync(item => item.Id == id);
		if (message == null) return false;

		message.Status = OutboxMessageStatus.Failed;
		message.LastError = error;
		message.LockedUntil = null;

		return await _context.SaveChangesAsync() > 0;
	}
}
