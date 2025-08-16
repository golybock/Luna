using System.Text;
using System.Text.Json;
using Luna.Tools.SharedModels.Models.Email;
using Luna.Tools.SharedModels.Models.RabbitMQ;
using RabbitMQ.Client;

namespace Luna.Auth.Services.Services.EmailService;

public class EmailService : IEmailService
{
	private readonly RabbitMQSettings _rabbitMqSettings;

	public EmailService(RabbitMQSettings rabbitMqSettings)
	{
		_rabbitMqSettings = rabbitMqSettings;
	}

	public async Task SendAuthCodeAsync(AuthCodeEmail authCodeEmail)
	{
		ConnectionFactory factory = new ConnectionFactory
		{
			HostName = _rabbitMqSettings.Host,
			Port = _rabbitMqSettings.Port,
			UserName = _rabbitMqSettings.Username,
			Password = _rabbitMqSettings.Password
		};
		await using IConnection connection = await factory.CreateConnectionAsync();
		await using IChannel channel = await connection.CreateChannelAsync();

		await channel.QueueDeclareAsync(
			queue: _rabbitMqSettings.Queue,
			durable: false,
			exclusive: false,
			autoDelete: false,
			arguments: null
		);

		byte[] body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(authCodeEmail));

		BasicProperties properties = new BasicProperties
		{
			Persistent = true, // Сообщение переживет перезагрузку сервера
			ContentType = "application/json",
			ContentEncoding = "utf-8",
			Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
			MessageId = Guid.NewGuid().ToString()
		};

		await channel.BasicPublishAsync(
			exchange: "",
			routingKey: _rabbitMqSettings.Queue,
			mandatory: false,
			basicProperties: properties,
			body: body
		);
	}
}