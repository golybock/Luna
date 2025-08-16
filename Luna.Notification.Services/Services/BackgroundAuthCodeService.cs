using System.Text;
using System.Text.Json;
using Luna.Tools.SharedModels.Models.Email;
using Luna.Tools.SharedModels.Models.RabbitMQ;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Luna.Notification.Services.Services;

public class BackgroundAuthCodeService : BackgroundService
{
	private readonly IConnection _connection;
	private readonly IChannel _channel;
	private readonly IEmailService _emailService;
	private string Host { get; }
	private int Port { get; set; }
	private string Queue { get; }
	private string Username { get; }
	private string Password { get; }


	public BackgroundAuthCodeService(IEmailService emailService, RabbitMQSettings rabbitMqSettings)
	{
		_emailService = emailService;

		Host = rabbitMqSettings.Host;
		Port = rabbitMqSettings.Port;
		Queue = rabbitMqSettings.Queue;
		Username = rabbitMqSettings.Username;
		Password = rabbitMqSettings.Password;

		ConnectionFactory factory = new ConnectionFactory
			{HostName = Host, Port = Port, Password = Password, UserName = Username};
		_connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
		_channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

		_channel.QueueDeclareAsync(
			queue: Queue,
			durable: false,
			exclusive: false,
			autoDelete: false,
			arguments: null).GetAwaiter().GetResult();
	}


	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel);

		consumer.ReceivedAsync += async (ch, ea) =>
		{
			Console.WriteLine("rabbitmq message received");
			try
			{
				string content = Encoding.UTF8.GetString(ea.Body.Span);

				AuthCodeEmail? authCodeEmail = JsonSerializer.Deserialize<AuthCodeEmail>(content);

				if (authCodeEmail != null)
				{
					await _emailService.SendAuthCodeEmailAsync(authCodeEmail);
				}
				else
				{
					Console.WriteLine("AuthCodeEmail could not be deserialized.");
				}

				await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false,
					cancellationToken: stoppingToken);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false,
					cancellationToken: stoppingToken);
			}
		};

		await _channel.BasicConsumeAsync(Queue, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

		await Task.Delay(Timeout.Infinite, stoppingToken);
	}

	public override async void Dispose()
	{
		try
		{
			if (_channel?.IsOpen == true)
			{
				await _channel.CloseAsync();
			}

			if (_connection?.IsOpen == true)
			{
				await _connection.CloseAsync();
			}
		}
		finally
		{
			_channel?.Dispose();
			_connection?.Dispose();
		}

		base.Dispose();
	}
}