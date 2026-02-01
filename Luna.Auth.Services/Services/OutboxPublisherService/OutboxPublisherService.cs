using System.Text.Json;
using Luna.Auth.Models.Domain.Models;
using Luna.Auth.Repositories.Repositories.OutboxRepository;
using Luna.Auth.Services.Services.EmailService;
using Luna.Tools.SharedModels.Models.Email;
using Luna.Tools.SharedModels.Models.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Luna.Auth.Services.Services.OutboxPublisherService;

public class OutboxPublisherService : BackgroundService
{
	private const int BatchSize = 20;
	private const int MaxAttempts = 10;
	private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);
	private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(1);
	private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(10);

	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<OutboxPublisherService> _logger;

	public OutboxPublisherService(IServiceProvider serviceProvider, ILogger<OutboxPublisherService> logger)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				using IServiceScope scope = _serviceProvider.CreateScope();
				IOutboxRepository outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
				IEmailService emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

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
					await ProcessMessageAsync(message, outboxRepository, emailService);
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

	private async Task ProcessMessageAsync(
		OutboxMessageDatabase message,
		IOutboxRepository outboxRepository,
		IEmailService emailService)
	{
		try
		{
			switch (message.Type)
			{
				case OutboxMessageTypes.AuthCodeEmail:
				{
					AuthCodeEmail? authCodeEmail = JsonSerializer.Deserialize<AuthCodeEmail>(message.Payload);
					if (authCodeEmail == null)
					{
						throw new InvalidOperationException("AuthCodeEmail payload is empty");
					}

					await emailService.SendAuthCodeAsync(authCodeEmail);
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
}
