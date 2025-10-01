using Luna.Pages.API.Hubs;
using Luna.Pages.Repositories.Context;
using Luna.Pages.Repositories.Repositories.Page.Command;
using Luna.Pages.Repositories.Repositories.Page.Query;
using Luna.Pages.Repositories.Repositories.PageVersion.Command;
using Luna.Pages.Repositories.Repositories.PageVersion.Query;
using Luna.Pages.Repositories.Repositories.WorkspaceUsers;
using Luna.Pages.Repositories.WorkspacePermissionRepository;
using Luna.Pages.Services.PermissionEventHandler;
using Luna.Pages.Services.Services;
using Luna.Pages.Services.Services.WorkspacePermissionService;
using Luna.Tools.Database.Npgsql.Options;
using Luna.Tools.Exception;
using Luna.Tools.SharedModels.Models.Kafka;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowCredentials()
			.SetIsOriginAllowed(origin => true);
	});
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddScoped<IWorkspacePermissionService, WorkspacePermissionService>();
builder.Services.AddScoped<IPermissionEventHandler, PermissionEventHandler>();

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddHostedService<PermissionEventConsumerService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<PageService>());

builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = "127.0.0.1:6379";
	options.InstanceName = "pages:";
});

builder.Services.AddSingleton<IWorkspacePermissionCacheRepository, WorkspacePermissionCacheRepository>(provider => new WorkspacePermissionCacheRepository(builder.Configuration.GetConnectionString("redis")));

DatabaseOptions databaseOptions = new DatabaseOptions()
{
	ConnectionString = builder.Configuration.GetConnectionString("luna_pages") ?? throw new InvalidOperationException()
};

builder.Services.AddDbContext<LunaPagesContext>(options =>
	options.UseNpgsql(databaseOptions.ConnectionString));

IConfigurationSection mongoSettings = builder.Configuration.GetSection("MongoDB");


string connectionString = mongoSettings.GetValue<string>("ConnectionString") ??
                          throw new InvalidOperationException("MongoDB connection string is required");

string databaseName = mongoSettings.GetValue<string>("DatabaseName") ??
                      throw new InvalidOperationException("MongoDB database name is required");
string pageCollectionName = mongoSettings.GetValue<string>("PagesCollectionName") ?? "page";
string pageVersionCollectionName = mongoSettings.GetValue<string>("PageVersionsCollectionName") ?? "page_versions";

builder.Services.AddScoped<IWorkspaceUserRepository, WorkspaceUserRepository>();

builder.Services.AddScoped<IPageQueryRepository>(provider =>
{
	ILogger<PageQueryRepository> logger = provider.GetRequiredService<ILogger<PageQueryRepository>>();
	return new PageQueryRepository(connectionString, databaseName, pageCollectionName, logger);
});

builder.Services.AddScoped<IPageCommandRepository>(provider =>
{
	ILogger<PageCommandRepository> logger = provider.GetRequiredService<ILogger<PageCommandRepository>>();
	return new PageCommandRepository(connectionString, databaseName, pageCollectionName, logger);
});

builder.Services.AddScoped<IPageVersionQueryRepository>(provider =>
{
	ILogger<PageVersionQueryRepository> logger = provider.GetRequiredService<ILogger<PageVersionQueryRepository>>();
	return new PageVersionQueryRepository(connectionString, databaseName, pageVersionCollectionName, logger);
});

builder.Services.AddScoped<IPageVersionCommandRepository>(provider =>
{
	ILogger<PageVersionCommandRepository> logger = provider.GetRequiredService<ILogger<PageVersionCommandRepository>>();
	return new PageVersionCommandRepository(connectionString, databaseName, pageVersionCollectionName, logger);
});

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors();

app.UseWebSockets();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<PageHub>("/ws/v1/pageHub");

app.MapControllers();

app.Run();