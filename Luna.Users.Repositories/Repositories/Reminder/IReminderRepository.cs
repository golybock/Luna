using Luna.Users.Models.Database.Models;

namespace Luna.Users.Repositories.Repositories.Reminder;

public interface IReminderRepository
{
	Task<IEnumerable<ReminderDatabase>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 50);
	Task<ReminderDatabase?> GetByIdAsync(Guid reminderId);
	Task<bool> CreateOrUpdateAsync(ReminderDatabase reminder);
	Task<bool> DeleteAsync(Guid reminderId);
	Task<bool> DeleteByUserIdAsync(Guid userId);
	Task<IEnumerable<ReminderDatabase>> GetDueRemindersAsync(DateTime dueDate);
	Task<int> GetCountByUserIdAsync(Guid userId);
}