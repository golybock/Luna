using System.Text.Json;
using Confluent.Kafka;
using Luna.Tools.SharedModels.Models.Kafka;
using Luna.Workspaces.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Luna.Workspaces.Services.Services.PermissionEventService;

public class PermissionEventService : IPermissionEventService, IDisposable
{
	private readonly IProducer<string, string> _producer;
	private readonly ILogger<PermissionEventService> _logger;
	private readonly KafkaSettings _kafkaSettings;

	private static string Key(WorkspaceUserPermission workspaceUserPermission) => "workspaceId:" +
		workspaceUserPermission.WorkspaceId + ":userId:" + workspaceUserPermission.UserId;

	private static string Key(Guid workspaceId, Guid userId) => "workspaceId:" + workspaceId + ":userId:" + userId;

	public PermissionEventService(
		ILogger<PermissionEventService> logger,
		IOptions<KafkaSettings> kafkaSettings)
	{
		_logger = logger;
		_kafkaSettings = kafkaSettings.Value;

		ProducerConfig config = new ProducerConfig
		{
			BootstrapServers = _kafkaSettings.BootstrapServers,
			ClientId = _kafkaSettings.ClientId,
			Acks = Acks.Leader,
			MessageTimeoutMs = 1000,
			RequestTimeoutMs = 5000,
			RetryBackoffMs = 100
		};

		_producer = new ProducerBuilder<string, string>(config)
			.SetErrorHandler((_, e) => _logger.LogError("Kafka producer error: {Error}", e.Reason))
			.Build();
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
		try
		{
			string message = JsonSerializer.Serialize(eventData, new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = false
			});

			Message<string, string> kafkaMessage = new Message<string, string>
			{
				Key = key,
				Value = message,
				Timestamp = new Timestamp(eventData.Timestamp)
			};

			DeliveryResult<string, string>? deliveryResult =
				await _producer.ProduceAsync(_kafkaSettings.PermissionEventsTopic, kafkaMessage);

			_logger.LogInformation(
				"Successfully published event {EventType} to Kafka topic {Topic}. Partition: {Partition}, Offset: {Offset}, Key: {Key}",
				eventData.EventType,
				_kafkaSettings.PermissionEventsTopic,
				deliveryResult.Partition,
				deliveryResult.Offset,
				key);
		}
		catch (ProduceException<string, string> ex)
		{
			_logger.LogError(ex,
				"Failed to publish event {EventType} to Kafka topic {Topic}. Key: {Key}, Error: {Error}",
				eventData.EventType,
				_kafkaSettings.PermissionEventsTopic,
				key,
				ex.Error?.Reason);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex,
				"Unexpected error publishing event {EventType} to Kafka topic {Topic}. Key: {Key}",
				eventData.EventType,
				_kafkaSettings.PermissionEventsTopic,
				key);
			throw;
		}
	}

	public void Dispose()
	{
		try
		{
			_producer.Flush(TimeSpan.FromSeconds(5));
			_producer.Dispose();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error disposing Kafka producer");
		}
	}
}