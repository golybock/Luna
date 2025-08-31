using System.Security.Claims;
using Luna.Auth.Repositories.Repositories.AuthRepository;
using Luna.Auth.Repositories.Repositories.SessionArchiveRepository;
using Luna.Auth.Repositories.Repositories.SessionRepository;
using Luna.Auth.Repositories.Repositories.VerificationCodeRepository;
using Luna.Auth.Services.Middleware;
using Luna.Auth.Services.Services.AccountManagementService;
using Luna.Auth.Services.Services.AuthService;
using Luna.Auth.Services.Services.EmailService;
using Luna.Auth.Services.Services.SessionManagementService;
using Luna.Auth.Services.Services.TokensService;
using Luna.Tools.Database.Npgsql.Options;
using Luna.Tools.Exception;
using Luna.Tools.SharedModels.Models.RabbitMQ;
using Luna.Tools.Web;
using Luna.Users.gRPC.Client.Services;
using Microsoft.AspNetCore.Authentication;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowAnyOrigin();
	});
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

JwtOptions jwtOptions = new JwtOptions(builder.Configuration);

builder.Services
	.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = "TokenAuth";
		options.DefaultSignInScheme = "Cookies";
		options.DefaultChallengeScheme = "TokenAuth";
	})
	.AddCookie("Cookies")
	.AddGoogle(options => {
		options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? throw new Exception("Google API Key not set");
		options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ?? throw new Exception("Google API Secret not set");
		options.SaveTokens = true;
		options.CallbackPath = new PathString("/api/v1/auth/signin-google");
		options.Scope.Add("email");
		options.Scope.Add("profile");
		options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
		options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
		options.ClaimActions.MapJsonKey("picture", "picture");
	})
	.AddTokenAuth("TokenAuth", options =>
	{
		options.Issuer = jwtOptions.Issuer;
		options.Audience = jwtOptions.Audience;
		options.SymmetricSecurityKey = jwtOptions.SymmetricSecurityKey;
		options.ValidInDays = jwtOptions.ValidInDays;
		options.RefreshValidInDays = jwtOptions.RefreshValidInDays;
	});


DatabaseOptions databaseOptions = new DatabaseOptions()
{
	ConnectionString = builder.Configuration.GetConnectionString("luna_auth") ?? throw new InvalidOperationException()
};

RabbitMQSettings? rabbitMqSettings = builder.Configuration.GetSection("RabbitMq").Get<RabbitMQSettings>();

builder.Services.AddSingleton(rabbitMqSettings ?? throw new NullReferenceException());
builder.Services.AddSingleton<IDatabaseOptions>(_ => databaseOptions);
builder.Services.AddSingleton(jwtOptions);

builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = "127.0.0.1:6379";
	options.InstanceName = "workspaces:";
});

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ISessionArchiveRepository, SessionArchiveRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>(provider => new SessionRepository(builder.Configuration.GetConnectionString("redis")));
builder.Services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>(provider => new VerificationCodeRepository(builder.Configuration.GetConnectionString("redis")));

builder.Services.AddScoped<IAccountManagementService, AccountManagementService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISessionManagementService, SessionManagementService>();
builder.Services.AddScoped<ITokensService, TokensService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddSingleton<IUserServiceClient>(_ => new UserServiceClient(builder.Configuration["gRPC:Host"]));

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();