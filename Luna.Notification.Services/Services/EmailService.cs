using System.Net;
using System.Net.Mail;
using System.Text;
using Luna.Tools.SharedModels.Models.Email;

namespace Luna.Notification.Services.Services;

public class EmailService : IEmailService
{
	private readonly EmailSettings _emailSettings;

	public EmailService(EmailSettings emailSettings)
	{
		_emailSettings = emailSettings;
	}

	public async Task<bool> SendAuthCodeEmailAsync(AuthCodeEmail emailData)
	{
		if (emailData == null || string.IsNullOrEmpty(emailData.RecipientEmail) ||
		    string.IsNullOrEmpty(emailData.AuthCode))
		{
			throw new ArgumentException("Email data is invalid");
		}

		string subject = $"Код авторизации для {emailData.AppName}";

		Console.WriteLine($"Code: {emailData.AuthCode} - {emailData.RecipientEmail}");

		string htmlBody = GenerateAuthCodeHtml(emailData);
		string plainTextBody = GenerateAuthCodePlainText(emailData);

		return await SendEmailAsync(emailData.RecipientEmail, subject, htmlBody, plainTextBody);
	}

	public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody,
		string? plainTextBody = null)
	{
		try
		{
			// todo переделать на logger
			Console.WriteLine($"Attempting to send email via {_emailSettings.SmtpHost}:{_emailSettings.SmtpPort}");
			Console.WriteLine($"SSL Enabled: {_emailSettings.EnableSsl}");
			Console.WriteLine($"From: {_emailSettings.SenderEmail}:{_emailSettings.SenderPassword}");
			Console.WriteLine($"To: {toEmail}");

			using SmtpClient client = new SmtpClient();
			client.Host = _emailSettings.SmtpHost;
			client.Port = _emailSettings.SmtpPort;
			client.Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
			client.EnableSsl = _emailSettings.EnableSsl;
			client.Timeout = 5000;

			if (_emailSettings.EnableSsl)
			{
				client.TargetName = "STARTTLS/" + _emailSettings.SmtpHost;
			}

			using MailMessage message = new MailMessage();
			message.From = new MailAddress(_emailSettings.SenderEmail);
			message.To.Add(toEmail);
			message.Subject = subject;
			message.IsBodyHtml = true;
			message.Body = htmlBody;
			message.BodyEncoding = Encoding.UTF8;
			message.SubjectEncoding = Encoding.UTF8;

			// Добавляем альтернативный plain text для клиентов, не поддерживающих HTML
			if (!string.IsNullOrEmpty(plainTextBody))
			{
				AlternateView plainView = AlternateView.CreateAlternateViewFromString(plainTextBody, Encoding.UTF8, "text/plain");
				message.AlternateViews.Add(plainView);
			}

			// todo переделать на logger
			Console.WriteLine("Sending email...");
			await client.SendMailAsync(message);
			Console.WriteLine("Email sent");
			return true;
		}
		catch (SmtpException smtpEx)
		{
			// todo переделать на logger
			Console.WriteLine($"SMTP Error: {smtpEx.Message}");
			Console.WriteLine($"Status Code: {smtpEx.StatusCode}");
			Console.WriteLine($"Full Exception: {smtpEx}");
			return false;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error sending email: {ex}");
			return false;
		}
	}

	private string GenerateAuthCodeHtml(AuthCodeEmail data)
	{
		// todo убрать такой путь
		string emailTemplate = File.ReadAllText("Resources/AuthCode.html");
		StringBuilder sb = new StringBuilder(emailTemplate);

		sb.Replace("{data.AppName}", data.AppName);
		sb.Replace("{data.AuthCode}", data.AuthCode);
		sb.Replace("{data.ExpirationMinutes}", data.ExpirationMinutes.ToString());
		sb.Replace("{data.RecipientName}", data.RecipientName);

		return sb.ToString();
	}

	private string GenerateAuthCodePlainText(AuthCodeEmail data)
	{
		// todo убрать такой путь
		string emailTemplate = File.ReadAllText("Resources/AuthCodePlain.txt");
		StringBuilder sb = new StringBuilder(emailTemplate);

		sb.Replace("{data.AppName}", data.AppName);
		sb.Replace("{data.AuthCode}", data.AuthCode);
		sb.Replace("{data.ExpirationMinutes}", data.ExpirationMinutes.ToString());
		sb.Replace("{data.RecipientName}", data.RecipientName);

		return sb.ToString();
	}
}