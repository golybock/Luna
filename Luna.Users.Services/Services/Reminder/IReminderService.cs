using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.View.Models;

namespace Luna.Users.Services.Services.Reminder;

public interface IReminderService
{
	Task<IEnumerable<ReminderView>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 50);
	Task<ReminderView?> GetByIdAsync(Guid reminderId);
	Task<bool> CreateOrUpdateAsync(Guid? reminderId, ReminderBlank reminder);
	Task<bool> DeleteAsync(Guid reminderId);
	Task<bool> DeleteByUserIdAsync(Guid userId);
	Task<IEnumerable<ReminderView>> GetDueRemindersAsync(DateTime dueDate);
	Task<int> GetCountByUserIdAsync(Guid userId);
}