using Luna.Auth.Repositories.Repositories.AuthRepository;
using Luna.Auth.Repositories.Repositories.SessionArchiveRepository;
using Luna.Auth.Repositories.Repositories.SessionRepository;
using Luna.Auth.Services.Middleware;
using Luna.Auth.Services.Middleware.Exception;
using Luna.Auth.Services.Services.AccountManagementService;
using Luna.Auth.Services.Services.AuthService;
using Luna.Auth.Services.Services.SessionManagementService;
using Luna.Auth.Services.Services.TokensService;
using Luna.Tools.Database.Npgsql.Options;
using Luna.Tools.Web;

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

if (builder.Environment.IsProduction())
{
	builder.Logging.AddJsonConsole(options =>
	{
		options.IncludeScopes = true;
		options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
	});
}

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
		options.Scope.Add("email"); // Запрашиваем email
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

builder.Services.AddSingleton<IDatabaseOptions>(_ => databaseOptions);
builder.Services.AddSingleton<JwtOptions>(jwtOptions);

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ISessionArchiveRepository, SessionArchiveRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>(provider => new SessionRepository(builder.Configuration.GetConnectionString("redis")));

builder.Services.AddScoped<IAccountManagementService, AccountManagementService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISessionManagementService, SessionManagementService>();
builder.Services.AddScoped<ITokensService, TokensService>();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();