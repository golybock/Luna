using Luna.Tools.Database.Npgsql.Options;
using Luna.Tools.Exception;
using Luna.Tools.SharedModels.Models.Kafka;
using Luna.Users.gRPC.Client.Services;
using Luna.Workspaces.Repositories.Context;
using Luna.Workspaces.Repositories.Repositories.InviteRepository;
using Luna.Workspaces.Repositories.Repositories.WorkspacePermissionRepository;
using Luna.Workspaces.Repositories.Repositories.WorkspaceRepository;
using Luna.Workspaces.Services.Services.InviteService;
using Luna.Workspaces.Services.Services.PermissionEventService;
using Luna.Workspaces.Services.Services.WorkspacePermissionService;
using Luna.Workspaces.Services.Services.WorkspaceService;
using Microsoft.EntityFrameworkCore;

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

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));

DatabaseOptions databaseOptions = new DatabaseOptions()
{
	ConnectionString = builder.Configuration.GetConnectionString("luna_workspaces") ?? throw new InvalidOperationException()
};

builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = "redis:6379,password=system";
	options.InstanceName = "workspaces:";
});

builder.Services.AddSingleton<IDatabaseOptions>(_ => databaseOptions);

builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
builder.Services.AddScoped<IWorkspacePermissionCacheRepository, WorkspacePermissionCacheRepository>(provider => new WorkspacePermissionCacheRepository(builder.Configuration.GetConnectionString("redis")));
builder.Services.AddScoped<IInviteRepository, InviteRepository>();

builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();
builder.Services.AddScoped<IWorkspacePermissionService, WorkspacePermissionService>();
builder.Services.AddScoped<IInviteService, InviteService>();

builder.Services.AddSingleton<IPermissionEventService, PermissionEventService>();

builder.Services.AddSingleton<IUserServiceClient>(_ => new UserServiceClient(builder.Configuration["gRPC:Host"]));

builder.Services.AddDbContext<LunaWorkspacesContext>(options =>
	options.UseNpgsql(databaseOptions.ConnectionString));


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