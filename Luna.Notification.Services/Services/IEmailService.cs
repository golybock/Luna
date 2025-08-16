using Luna.Tools.SharedModels.Models.Email;

namespace Luna.Notification.Services.Services;

public interface IEmailService
{
	Task<bool> SendAuthCodeEmailAsync(AuthCodeEmail emailData);
	Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, string? plainTextBody = null);
}