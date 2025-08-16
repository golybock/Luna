using Luna.Users.Models.Database.Models;
using Luna.Users.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Luna.Users.Repositories.Repositories.Reminder;

public class ReminderRepository : IReminderRepository
{
	private readonly LunaUsersContext _context;

	public ReminderRepository(LunaUsersContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<ReminderDatabase>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 50)
	{
		return await _context.Reminders
			.AsNoTracking()
			.Where(r => r.UserId == userId)
			.OrderBy(r => r.DueAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();
	}

	public async Task<ReminderDatabase?> GetByIdAsync(Guid reminderId)
	{
		return await _context.Reminders
			.AsNoTracking()
			.FirstOrDefaultAsync(r => r.Id == reminderId);
	}

	public async Task<bool> CreateOrUpdateAsync(ReminderDatabase reminder)
	{
		ReminderDatabase? existing = await _context.Reminders
			.FirstOrDefaultAsync(r => r.Id == reminder.Id);

		if (existing == null)
		{
			reminder.Id = Guid.NewGuid();
			reminder.CreatedAt = DateTime.UtcNow;
			reminder.UpdatedAt = DateTime.UtcNow;

			_context.Reminders.Add(reminder);
		}
		else
		{
			existing.Title = reminder.Title;
			existing.Description = reminder.Description;
			existing.DueAt = reminder.DueAt;
			existing.Status = reminder.Status;
			existing.NotificationSent = reminder.NotificationSent;
			existing.RepeatRule = reminder.RepeatRule;
			existing.UpdatedAt = DateTime.UtcNow;
		}

		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> DeleteAsync(Guid reminderId)
	{
		int deleted = await _context.Reminders
			.Where(r => r.Id == reminderId)
			.ExecuteDeleteAsync();

		return deleted > 0;
	}

	public async Task<bool> DeleteByUserIdAsync(Guid userId)
	{
		int deleted = await _context.Reminders
			.Where(r => r.UserId == userId)
			.ExecuteDeleteAsync();

		return deleted > 0;
	}

	public async Task<IEnumerable<ReminderDatabase>> GetDueRemindersAsync(DateTime dueDate)
	{
		return await _context.Reminders
			.AsNoTracking()
			.Where(r => r.DueAt <= dueDate && !r.NotificationSent && r.Status == 1)
			.ToListAsync();
	}

	public async Task<int> GetCountByUserIdAsync(Guid userId)
	{
		return await _context.Reminders
			.AsNoTracking()
			.CountAsync(r => r.UserId == userId);
	}
}