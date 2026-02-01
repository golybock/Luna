using System.Text.Json;
using Confluent.Kafka;
using Luna.Tools.SharedModels.Models.Kafka;
using Luna.Tools.SharedModels.Models.Outbox;
using Luna.Workspaces.Domain.Models;
using Luna.Workspaces.Repositories.Repositories.OutboxRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Luna.Workspaces.Services.Services.OutboxPublisherService;

public class OutboxPublisherService : BackgroundService
{
	private const int BatchSize = 50;
	private const int MaxAttempts = 10;
	private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);
	private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(1);
	private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(10);

	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<OutboxPublisherService> _logger;
	private readonly KafkaSettings _kafkaSettings;
	private IProducer<string, string>? _producer;

	public OutboxPublisherService(
		IServiceProvider serviceProvider,
		ILogger<OutboxPublisherService> logger,
		IOptions<KafkaSettings> kafkaSettings)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
		_kafkaSettings = kafkaSettings.Value;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		InitializeProducer();

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				using IServiceScope scope = _serviceProvider.CreateScope();
				IOutboxRepository outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();

				DateTime lockUntil = DateTime.UtcNow.Add(LockDuration);
				IReadOnlyList<OutboxMessageDatabase> messages =
					await outboxRepository.AcquirePendingMessagesAsync(BatchSize, lockUntil);

				if (messages.Count == 0)
				{
					await Task.Delay(PollInterval, stoppingToken);
					continue;
				}

				foreach (OutboxMessageDatabase message in messages)
				{
					await ProcessMessageAsync(message, outboxRepository);
				}
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Outbox publisher loop error");
				await Task.Delay(PollInterval, stoppingToken);
			}
		}
	}

	private void InitializeProducer()
	{
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

	private async Task ProcessMessageAsync(OutboxMessageDatabase message, IOutboxRepository outboxRepository)
	{
		try
		{
			switch (message.Type)
			{
				case OutboxMessageTypes.PermissionEvent:
				{
					PermissionEventOutboxPayload? payload =
						JsonSerializer.Deserialize<PermissionEventOutboxPayload>(message.Payload, new JsonSerializerOptions
						{
							PropertyNamingPolicy = JsonNamingPolicy.CamelCase
						});

					if (payload?.Event == null || string.IsNullOrWhiteSpace(payload.Key))
					{
						throw new InvalidOperationException("PermissionEvent payload is invalid");
					}

					string eventMessage = JsonSerializer.Serialize(payload.Event, new JsonSerializerOptions
					{
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
						WriteIndented = false
					});

					Message<string, string> kafkaMessage = new Message<string, string>
					{
						Key = payload.Key,
						Value = eventMessage,
						Timestamp = new Timestamp(payload.Event.Timestamp)
					};

					await _producer!.ProduceAsync(_kafkaSettings.PermissionEventsTopic, kafkaMessage);
					break;
				}
				default:
					throw new InvalidOperationException($"Unknown outbox message type: {message.Type}");
			}

			await outboxRepository.MarkProcessedAsync(message.Id, DateTime.UtcNow);
		}
		catch (Exception ex)
		{
			string error = ex.Message.Length > 1000 ? ex.Message[..1000] : ex.Message;

			if (message.Attempts >= MaxAttempts)
			{
				await outboxRepository.MarkFailedAsync(message.Id, error);
				_logger.LogError(ex, "Outbox message {MessageId} failed permanently", message.Id);
				return;
			}

			DateTime retryAt = DateTime.UtcNow.Add(RetryDelay);
			await outboxRepository.MarkForRetryAsync(message.Id, error, retryAt);
			_logger.LogWarning(ex, "Outbox message {MessageId} failed, scheduled retry", message.Id);
		}
	}

	public override void Dispose()
	{
		try
		{
			_producer?.Flush(TimeSpan.FromSeconds(5));
			_producer?.Dispose();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error disposing Kafka producer");
		}

		base.Dispose();
	}
}
