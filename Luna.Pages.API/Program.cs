using Luna.Pages.API.Hubs;
using Luna.Pages.Repositories.Repositories.Page.Command;
using Luna.Pages.Repositories.Repositories.Page.Query;
using Luna.Pages.Repositories.Repositories.PageComment;
using Luna.Pages.Repositories.Repositories.PageVersion.Command;
using Luna.Pages.Services.Services;
using Luna.Tools.Exception;
using MediatR;
using MongoDB.Bson.Serialization.Conventions;

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
			.AllowAnyOrigin();
	});
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();


// builder.Logging.AddJsonConsole(options =>
// {
// 	options.IncludeScopes = true;
// 	options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
// });

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<PageService>());

IConfigurationSection mongoSettings = builder.Configuration.GetSection("MongoDB");
string connectionString = mongoSettings.GetValue<string>("ConnectionString") ??
                          throw new InvalidOperationException("MongoDB connection string is required");
string databaseName = mongoSettings.GetValue<string>("DatabaseName") ??
                      throw new InvalidOperationException("MongoDB database name is required");
string pageCollectionName = mongoSettings.GetValue<string>("PagesCollectionName") ?? "page";
string pageVersionCollectionName = mongoSettings.GetValue<string>("PageVersionsCollectionName") ?? "page_versions";

// page
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

builder.Services.AddScoped<IPageVersionCommandRepository>(provider =>
{
	ILogger<PageVersionCommandRepository> logger = provider.GetRequiredService<ILogger<PageVersionCommandRepository>>();
	return new PageVersionCommandRepository(connectionString, databaseName, pageVersionCollectionName, logger);
});

builder.Services.AddScoped<IPageService, PageService>();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// app.UseEndpoints(endpoints =>
// {
// 	endpoints.MapHub<PageHub>("/page");
// });

app.MapControllers();

app.Run();