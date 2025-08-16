using Luna.Tools.SharedModels.Models;
using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.Database.Models;
using Luna.Users.Models.Domain.Models;
using Luna.Users.Models.View.Models;

namespace Luna.Users.Services.Extensions;

public static class ReminderExtension
{
		public static ReminderView ToView(this ReminderDatabase reminder)
	{
		return new ReminderView()
		{
			Id = reminder.Id,
			EntityId = reminder.EntityId,
			EntityType = reminder.EntityType,
			DueAt = reminder.DueAt,
			Title = reminder.Title,
			Description = reminder.Description,
			Status = (ReminderStatus)reminder.Status,
			RepeatRule = reminder.RepeatRule
		};
	}

	public static ReminderView ToView(this ReminderDomain reminder)
	{
		return new ReminderView()
		{
			Id = reminder.Id,
			EntityId = reminder.EntityId,
			EntityType = reminder.EntityType,
			DueAt = reminder.DueAt,
			Title = reminder.Title,
			Description = reminder.Description,
			Status = (ReminderStatus)reminder.Status,
			RepeatRule = reminder.RepeatRule
		};
	}

	public static ReminderDomain ToDomain(this ReminderDatabase reminder)
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

	public static ReminderDomain ToDomain(this ReminderBlank reminder)
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

	public static ReminderDatabase ToDatabase(this ReminderDomain reminder)
	{
		return new ReminderDatabase()
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

}