using Luna.Tools.SharedModels.Models;
using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.Database.Models;
using Luna.Users.Models.View.Models;

namespace Luna.Users.Models.Domain.Models;

public class ReminderDomain
{
	public Guid Id { get; set; }

	public Guid UserId { get; set; }

	public Guid EntityId { get; set; }

	public int EntityType { get; set; }

	public DateTime DueAt { get; set; }

	public string Title { get; set; } = null!;

	public string? Description { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }

	public int Status { get; set; }

	public bool NotificationSent { get; set; }

	public string? RepeatRule { get; set; }

	public static ReminderDomain FromBlank(ReminderBlank reminder)
	{
		return new ReminderDomain()
		{
			EntityId = reminder.EntityId,
			DueAt = reminder.DueAt,
			Title = reminder.Title,
			Description = reminder.Description,
			Status = reminder.Status,
			RepeatRule = reminder.RepeatRule
		};
	}

	public static ReminderDomain FromDatabase(ReminderDatabase reminder)
	{
		return new ReminderDomain()
		{
			Id = reminder.Id,
			UserId = reminder.UserId,
			EntityId = reminder.EntityId,
			EntityType = reminder.EntityType,
			DueAt = reminder.DueAt,
			Title = reminder.Title,
			Description = reminder.Description,
			CreatedAt = reminder.CreatedAt,
			UpdatedAt = reminder.UpdatedAt,
			Status = reminder.Status,
			NotificationSent = reminder.NotificationSent,
			RepeatRule = reminder.RepeatRule
		};
	}

	public ReminderDatabase ToDatabase()
	{
		return new ReminderDatabase()
		{
			Id = Id,
			UserId = UserId,
			EntityId = EntityId,
			EntityType = EntityType,
			DueAt = DueAt,
			Title = Title,
			Description = Description,
			CreatedAt = CreatedAt,
			UpdatedAt = UpdatedAt,
			Status = Status,
			NotificationSent = NotificationSent,
			RepeatRule = RepeatRule
		};
	}

	public ReminderView ToView()
	{
		return new ReminderView()
		{
			Id = Id,
			EntityId = EntityId,
			EntityType = EntityType,
			DueAt = DueAt,
			Title = Title,
			Description = Description,
			Status = (ReminderStatus)Status,
			RepeatRule = RepeatRule
		};
	}
}