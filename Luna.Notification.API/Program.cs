using Luna.Notification.Services.Services;
using Luna.Tools.SharedModels.Models.Email;
using Luna.Tools.SharedModels.Models.RabbitMQ;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowAnyOrigin();
	});
});

RabbitMQSettings? rabbitMqSettings = builder.Configuration.GetSection("RabbitMq").Get<RabbitMQSettings>();
EmailSettings? emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>();

if (emailSettings == null) throw new NullReferenceException("Email settings not passed");

emailSettings.SenderName = Environment.GetEnvironmentVariable("SENDER_NAME");
emailSettings.SenderEmail = Environment.GetEnvironmentVariable("SENDER_EMAIL");
emailSettings.SenderPassword = Environment.GetEnvironmentVariable("SENDER_PASSWORD");

builder.Services.AddSingleton(rabbitMqSettings ?? throw new NullReferenceException());
builder.Services.AddSingleton(emailSettings ?? throw new NullReferenceException());
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHostedService<BackgroundAuthCodeService>(_ => new BackgroundAuthCodeService(new EmailService(emailSettings), rabbitMqSettings));

WebApplication app = builder.Build();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();