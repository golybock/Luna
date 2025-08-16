using Luna.Tools.SharedModels.Models.Email;

namespace Luna.Auth.Services.Services.EmailService;

public interface IEmailService
{
	Task SendAuthCodeAsync(AuthCodeEmail authCodeEmail);
}