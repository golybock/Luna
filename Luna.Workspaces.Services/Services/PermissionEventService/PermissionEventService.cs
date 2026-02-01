using System.Text.Json;
using Luna.Tools.SharedModels.Models.Kafka;
using Luna.Tools.SharedModels.Models.Outbox;
using Luna.Workspaces.Domain.Models;
using Luna.Workspaces.Repositories.Repositories.OutboxRepository;
using Microsoft.Extensions.Logging;

namespace Luna.Workspaces.Services.Services.PermissionEventService;

public class PermissionEventService : IPermissionEventService
{
	private readonly ILogger<PermissionEventService> _logger;
	private readonly IOutboxRepository _outboxRepository;

	private static string Key(WorkspaceUserPermission workspaceUserPermission) => "workspaceId:" +
		workspaceUserPermission.WorkspaceId + ":userId:" + workspaceUserPermission.UserId;

	private static string Key(Guid workspaceId, Guid userId) => "workspaceId:" + workspaceId + ":userId:" + userId;

	public PermissionEventService(
		ILogger<PermissionEventService> logger,
		IOutboxRepository outboxRepository)
	{
		_logger = logger;
		_outboxRepository = outboxRepository;
	}

	public async Task CreateWorkspaceUserPermissionsAsync(WorkspaceUserPermission workspaceUserPermission)
	{
		PermissionEvent eventData = new PermissionEvent
		{
			EventType = PermissionEventType.Created,
			Timestamp = DateTime.UtcNow,
			Data = workspaceUserPermission
		};

		await PublishEventAsync(eventData, Key(workspaceUserPermission));
	}

	public async Task UpdateWorkspaceUserPermissions(WorkspaceUserPermission workspaceUserPermission)
	{
		PermissionEvent eventData = new PermissionEvent
		{
			EventType = PermissionEventType.Updated,
			Timestamp = DateTime.UtcNow,
			Data = workspaceUserPermission
		};

		await PublishEventAsync(eventData, Key(workspaceUserPermission));
	}

	public async Task DeleteWorkspaceUserPermissionsById(Guid workspaceId, Guid userId)
	{
		string id = Key(workspaceId, userId);

		PermissionEvent eventData = new PermissionEvent
		{
			EventType = PermissionEventType.DeletedById,
			Timestamp = DateTime.UtcNow,
			Data = new {Id = id}
		};

		await PublishEventAsync(eventData, id);
	}

	public async Task DeleteWorkspaceUserPermissionsByWorkspaceId(Guid workspaceId)
	{
		PermissionEvent eventData = new PermissionEvent
		{
			EventType = PermissionEventType.DeletedByWorkspaceId,
			Timestamp = DateTime.UtcNow,
			Data = new {WorkspaceId = workspaceId}
		};

		await PublishEventAsync(eventData, workspaceId.ToString());
	}

	public async Task DeleteWorkspaceUserPermissionsByUserId(Guid userId)
	{
		PermissionEvent eventData = new PermissionEvent
		{
			EventType = PermissionEventType.DeletedByUserId,
			Timestamp = DateTime.UtcNow,
			Data = new {UserId = userId}
		};

		await PublishEventAsync(eventData, userId.ToString());
	}

	private async Task PublishEventAsync(PermissionEvent eventData, string key)
	{
		PermissionEventOutboxPayload payload = new PermissionEventOutboxPayload
		{
			Key = key,
			Event = eventData
		};

		string message = JsonSerializer.Serialize(payload, new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = false
		});

		OutboxMessageDatabase outboxMessage = new OutboxMessageDatabase
		{
			Id = Guid.NewGuid(),
			Type = OutboxMessageTypes.PermissionEvent,
			Payload = message,
			Status = OutboxMessageStatus.Pending,
			Attempts = 0,
			CreatedAt = DateTime.UtcNow
		};

		bool saved = await _outboxRepository.AddMessageAsync(outboxMessage);
		if (!saved)
		{
			_logger.LogError("Failed to save outbox message for permission event {EventType}", eventData.EventType);
			throw new Exception("Failed to save outbox message");
		}
	}
}