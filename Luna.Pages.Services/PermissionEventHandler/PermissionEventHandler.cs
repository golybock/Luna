using System.Text.Json;
using Luna.Pages.Models.Domain.Models;
using Luna.Pages.Services.Services.WorkspacePermissionService;
using Luna.Tools.SharedModels.Models.Kafka;
using Microsoft.Extensions.Logging;

namespace Luna.Pages.Services.PermissionEventHandler;

public class PermissionEventHandler : IPermissionEventHandler
{
	private readonly ILogger<PermissionEventHandler> _logger;
	private readonly IWorkspacePermissionService _workspacePermissionService;

	public PermissionEventHandler(
		ILogger<PermissionEventHandler> logger,
		IWorkspacePermissionService workspacePermissionService
	)
	{
		_logger = logger;
		_workspacePermissionService = workspacePermissionService;
	}

	public async Task HandleAsync(PermissionEvent permissionEvent)
	{
		_logger.LogInformation("Processing permission event: {EventType} at {Timestamp}",
			permissionEvent.EventType, permissionEvent.Timestamp);

		try
		{
			switch (permissionEvent.EventType)
			{
				case PermissionEventType.Created:
					await HandleCreatedEvent(permissionEvent);
					break;
				case PermissionEventType.Updated:
					await HandleUpdatedEvent(permissionEvent);
					break;
				case PermissionEventType.DeletedById:
					await HandleDeletedByIdEvent(permissionEvent);
					break;
				case PermissionEventType.DeletedByWorkspaceId:
					await HandleDeletedByWorkspaceIdEvent(permissionEvent);
					break;
				case PermissionEventType.DeletedByUserId:
					await HandleDeletedByUserIdEvent(permissionEvent);
					break;
				default:
					_logger.LogWarning("Unknown event type: {EventType}", permissionEvent.EventType);
					break;
			}

			_logger.LogInformation("Successfully processed permission event: {EventType}", permissionEvent.EventType);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error processing permission event: {EventType}", permissionEvent.EventType);
			throw;
		}
	}

	private async Task HandleCreatedEvent(PermissionEvent permissionEvent)
	{
		try
		{
			if (permissionEvent.Data is JsonElement jsonElement)
			{
				WorkspaceUserPermission? workspaceUserPermission = jsonElement.Deserialize<WorkspaceUserPermission>();

				if (workspaceUserPermission == null)
				{
					throw new Exception("Invalid permission event");
				}

				await _workspacePermissionService.AddUserToWorkspaceAsync(workspaceUserPermission);

				_logger.LogDebug("Created event processed for data: {Data}",
					JsonSerializer.Serialize(permissionEvent.Data));
			}
			else
			{
				_logger.LogError("Event data is not a JsonElement");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error processing HandleCreatedEvent event");
			throw;
		}
	}

	private async Task HandleUpdatedEvent(PermissionEvent permissionEvent)
	{
		try
		{
			if (permissionEvent.Data is JsonElement jsonElement)
			{
				WorkspaceUserPermission? workspaceUserPermission = jsonElement.Deserialize<WorkspaceUserPermission>();

				if (workspaceUserPermission == null)
				{
					throw new Exception("Invalid permission event");
				}

				await _workspacePermissionService.UpdateUserWorkspace(workspaceUserPermission);

				_logger.LogDebug("Updated event processed for data: {Data}",
					JsonSerializer.Serialize(permissionEvent.Data));
			}
			else
			{
				_logger.LogError("Event data is not a JsonElement");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error processing HandleUpdatedEvent event");
			throw;
		}
	}

	private async Task HandleDeletedByIdEvent(PermissionEvent permissionEvent)
	{
		if (permissionEvent.Data is JsonElement jsonElement)
		{
			if (jsonElement.TryGetProperty("workspaceId", out JsonElement workspaceIdElement) &&
			    jsonElement.TryGetProperty("userId", out JsonElement userIdElement))
			{
				Guid workspaceId = workspaceIdElement.GetGuid();
				Guid userId = userIdElement.GetGuid();

				_logger.LogInformation("Processing DeletedById event for WorkspaceId: {WorkspaceId}", workspaceId);

				await _workspacePermissionService.DeleteUserFromWorkspaceAsync(workspaceId, userId);
			}
			else
			{
				_logger.LogError("WorkspaceId or UserId property not found in event data");
			}
		}
		else
		{
			_logger.LogError("Event data is not a JsonElement");
		}
	}

	private async Task HandleDeletedByWorkspaceIdEvent(PermissionEvent permissionEvent)
	{
		if (permissionEvent.Data is JsonElement jsonElement)
		{
			if (jsonElement.TryGetProperty("workspaceId", out JsonElement workspaceIdElement))
			{
				Guid workspaceId = workspaceIdElement.GetGuid();

				_logger.LogInformation("Processing DeletedByWorkspaceId event for WorkspaceId: {WorkspaceId}",
					workspaceId);

				await _workspacePermissionService.DeleteUserFromWorkspaceByWorkspaceIdAsync(workspaceId);
			}
			else
			{
				_logger.LogError("WorkspaceId property not found in event data");
			}
		}
		else
		{
			_logger.LogError("Event data is not a JsonElement");
		}
	}

	private async Task HandleDeletedByUserIdEvent(PermissionEvent permissionEvent)
	{
		if (permissionEvent.Data is JsonElement jsonElement)
		{
			if (jsonElement.TryGetProperty("userId", out JsonElement userIdElement))
			{
				Guid userId = userIdElement.GetGuid();

				_logger.LogInformation("Processing DeletedByUserId event for UserId: {UserId}", userId);

				await _workspacePermissionService.DeleteUserFromWorkspaceByUserIdAsync(userId);
			}
			else
			{
				_logger.LogError("UserId property not found in event data");
			}
		}
		else
		{
			_logger.LogError("Event data is not a JsonElement");
		}
	}
}