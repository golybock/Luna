using Luna.Tools.Database.Npgsql.Options;
using Luna.Users.gRPC.Services;
using Luna.Users.Repositories.Context;
using Luna.Users.Repositories.Repositories.User;
using Luna.Users.Services.Services.User;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddGrpc(options => { options.EnableDetailedErrors = true; });

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

DatabaseOptions databaseOptions = new DatabaseOptions()
{
	ConnectionString = builder.Configuration.GetConnectionString("luna_users") ?? throw new InvalidOperationException()
};

builder.Services.AddSingleton<IDatabaseOptions>(_ => databaseOptions);
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddDbContext<LunaUsersContext>(options =>
	options.UseNpgsql(databaseOptions.ConnectionString));

builder.Services.AddScoped<IUserService, UserService>();

WebApplication app = builder.Build();

app.MapGrpcService<UserGrpcService>();

app.MapGet("/",
	() =>
		"Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
