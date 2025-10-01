using System.Diagnostics;
using System.Text.Json;
using Confluent.Kafka;
using Luna.Tools.SharedModels.Models.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Luna.Pages.Services.PermissionEventHandler;

public class PermissionEventConsumerService : BackgroundService
{
	private readonly ILogger<PermissionEventConsumerService> _logger;
	private readonly KafkaSettings _kafkaSettings;
	private readonly IServiceProvider _serviceProvider;
	private IConsumer<string, string>? _consumer;
	private const int MaxRetryAttempts = 10;
	private const int RetryDelaySeconds = 5;

	public PermissionEventConsumerService(
		ILogger<PermissionEventConsumerService> logger,
		IOptions<KafkaSettings> kafkaSettings,
		IServiceProvider serviceProvider
	)
	{
		_logger = logger;
		_kafkaSettings = kafkaSettings.Value;
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Starting Permission Event Consumer Service");
		_logger.LogInformation("Kafka Settings: {Settings}", JsonSerializer.Serialize(_kafkaSettings));

		int retryCount = 0;

		while (!stoppingToken.IsCancellationRequested && retryCount < MaxRetryAttempts)
		{
			try
			{
				await InitializeConsumerAsync(stoppingToken);
				break;
			}
			catch (Exception ex)
			{
				retryCount++;
				_logger.LogWarning(ex,
					"Failed to initialize Kafka consumer (attempt {RetryCount}/{MaxRetries}). Retrying in {Delay} seconds...",
					retryCount, MaxRetryAttempts, RetryDelaySeconds);

				if (retryCount >= MaxRetryAttempts)
				{
					_logger.LogError("Failed to initialize Kafka consumer after {MaxRetries} attempts. Service will not process events",
						MaxRetryAttempts);
					return;
				}

				await Task.Delay(TimeSpan.FromSeconds(RetryDelaySeconds), stoppingToken);
			}
		}

		if (_consumer == null)
		{
			_logger.LogError("Consumer is null after initialization. Exiting service");
			return;
		}

		await ConsumeMessagesAsync(stoppingToken);
	}

	private async Task InitializeConsumerAsync(CancellationToken stoppingToken)
	{
		ConsumerConfig config = new ConsumerConfig
		{
			BootstrapServers = _kafkaSettings.BootstrapServers,
			GroupId = _kafkaSettings.ConsumerGroupId,
			AutoOffsetReset = AutoOffsetReset.Earliest,
			EnableAutoCommit = false,
			SessionTimeoutMs = 30000,
			MaxPollIntervalMs = 300000,
			EnablePartitionEof = false,
			ClientId = $"{_kafkaSettings.ClientId}-consumer",
			StatisticsIntervalMs = 5000,
			SocketTimeoutMs = 10000,
			ApiVersionRequest = true
		};

		_consumer = new ConsumerBuilder<string, string>(config)
			.SetErrorHandler((_, e) =>
			{
				if (e.IsFatal)
					_logger.LogError("Fatal consumer error: {Error} - {Reason}", e.Code, e.Reason);
				else
					_logger.LogWarning("Consumer error: {Error} - {Reason}", e.Code, e.Reason);
			})
			.SetPartitionsAssignedHandler((c, partitions) =>
			{
				_logger.LogInformation("Assigned partitions: [{Partitions}]",
					string.Join(", ", partitions.Select(p => $"{p.Topic}:{p.Partition}")));
			})
			.SetPartitionsRevokedHandler((c, partitions) =>
			{
				_logger.LogInformation("Revoked partitions: [{Partitions}]",
					string.Join(", ", partitions.Select(p => $"{p.Topic}:{p.Partition}")));
			})
			.SetStatisticsHandler((_, json) => { _logger.LogDebug("Consumer statistics: {Statistics}", json); })
			.Build();

		_consumer.Subscribe(_kafkaSettings.PermissionEventsTopic);

		_logger.LogInformation("Successfully initialized Kafka consumer for topic: {Topic} with group: {GroupId}",
			_kafkaSettings.PermissionEventsTopic, _kafkaSettings.ConsumerGroupId);

		// Проверяем подключение, сделав пробный consume
		ConsumeResult<string, string>? testResult = _consumer.Consume(TimeSpan.FromSeconds(5));
		_logger.LogInformation("Kafka consumer connection test successful");

		await Task.CompletedTask;
	}

	private async Task ConsumeMessagesAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Started consuming permission events from topic: {Topic}",
			_kafkaSettings.PermissionEventsTopic);

		try
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					ConsumeResult<string, string>? consumeResult = _consumer!.Consume(TimeSpan.FromSeconds(1));

					if (consumeResult?.Message == null) continue;

					await ProcessMessageAsync(consumeResult, stoppingToken);

					try
					{
						_consumer.Commit(consumeResult);
						_logger.LogDebug("Committed offset {Offset} for partition {Partition}",
							consumeResult.Offset, consumeResult.Partition);
					}
					catch (KafkaException commitEx)
					{
						_logger.LogError(commitEx, "Failed to commit offset {Offset} for partition {Partition}",
							consumeResult.Offset, consumeResult.Partition);
					}
				}
				catch (ConsumeException consumeEx)
				{
					_logger.LogError(consumeEx, "Error consuming message from Kafka: {Error}", consumeEx.Error?.Reason);

					if (consumeEx.Error?.IsFatal == true)
					{
						_logger.LogCritical("Fatal error encountered. Attempting to reconnect...");
						await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

						// Попытка переподключения
						await CleanupConsumerAsync();
						await InitializeConsumerAsync(stoppingToken);
					}
				}
				catch (OperationCanceledException)
				{
					_logger.LogInformation("Consumer operation was cancelled");
					break;
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Unexpected error while consuming messages");
					await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
				}
			}
		}
		catch (OperationCanceledException)
		{
			_logger.LogInformation("Consumer service was cancelled");
		}
		finally
		{
			await CleanupConsumerAsync();
		}
	}

	private async Task ProcessMessageAsync(ConsumeResult<string, string> consumeResult, CancellationToken stoppingToken)
	{
		Message<string, string>? message = consumeResult.Message;
		Stopwatch stopwatch = Stopwatch.StartNew();

		_logger.LogDebug("Processing message with key: {Key}, partition: {Partition}, offset: {Offset}",
			message.Key, consumeResult.Partition, consumeResult.Offset);

		try
		{
			PermissionEvent? permissionEvent = DeserializeEvent(message.Value);
			if (permissionEvent == null)
			{
				_logger.LogWarning("Skipping null permission event from message with key: {Key}", message.Key);
				return;
			}

			if (!IsValidEvent(permissionEvent))
			{
				_logger.LogWarning("Skipping invalid permission event {EventType} from message with key: {Key}",
					permissionEvent.EventType, message.Key);
				return;
			}

			// Создаем scope для получения scoped сервисов
			using IServiceScope scope = _serviceProvider.CreateScope();
			IPermissionEventHandler permissionEventHandler = scope.ServiceProvider.GetRequiredService<IPermissionEventHandler>();

			await permissionEventHandler.HandleAsync(permissionEvent);

			stopwatch.Stop();
			_logger.LogInformation(
				"Successfully processed permission event {EventType} with key {Key} in {ElapsedMs}ms",
				permissionEvent.EventType, message.Key, stopwatch.ElapsedMilliseconds);
		}
		catch (JsonException jsonEx)
		{
			stopwatch.Stop();
			_logger.LogError(jsonEx, "Failed to deserialize permission event from message with key {Key}: {Message}",
				message.Key, message.Value);
		}
		catch (Exception ex)
		{
			stopwatch.Stop();
			_logger.LogError(ex, "Failed to process permission event with key {Key} after {ElapsedMs}ms",
				message.Key, stopwatch.ElapsedMilliseconds);

			// Не пробрасываем исключение дальше, чтобы не остановить consumer
			_logger.LogWarning("Continuing to process next message despite error");
		}
	}

	private PermissionEvent? DeserializeEvent(string messageValue)
	{
		try
		{
			return JsonSerializer.Deserialize<PermissionEvent>(messageValue, new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				PropertyNameCaseInsensitive = true
			});
		}
		catch (JsonException ex)
		{
			_logger.LogError(ex, "JSON deserialization failed for message: {Message}", messageValue);
			return null;
		}
	}

	private bool IsValidEvent(PermissionEvent permissionEvent)
	{
		if (permissionEvent.Timestamp == default)
		{
			_logger.LogWarning("Permission event has default timestamp");
			return false;
		}

		if (Enum.IsDefined(typeof(PermissionEventType), permissionEvent.EventType)) return true;
		_logger.LogWarning("Permission event has undefined event type: {EventType}", permissionEvent.EventType);
		return false;
	}

	private async Task CleanupConsumerAsync()
	{
		if (_consumer != null)
		{
			try
			{
				_logger.LogInformation("Closing Kafka consumer");
				_consumer.Close();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error closing Kafka consumer");
			}
			finally
			{
				_consumer.Dispose();
				_consumer = null;
			}
		}

		await Task.CompletedTask;
	}

	public override async Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Stopping Permission Event Consumer Service");
		await base.StopAsync(cancellationToken);
		await CleanupConsumerAsync();
		_logger.LogInformation("Permission Event Consumer Service stopped");
	}

	public override void Dispose()
	{
		CleanupConsumerAsync().GetAwaiter().GetResult();
		base.Dispose();
		GC.SuppressFinalize(this);
	}
}