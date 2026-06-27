using Luna.Gateway.API.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

string redisConnectionString = builder.Configuration.GetConnectionString("redis") ?? "redis:6379,abortConnect=false";
builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(sp => 
	StackExchange.Redis.ConnectionMultiplexer.Connect(redisConnectionString));

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration).AddDelegatingHandler<CookieForwardingDelegatingHandler>(true);
;

builder.Services.AddHttpClient("AuthValidation")
	.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
	{
		UseCookies = true,
		CookieContainer = new System.Net.CookieContainer(),
		AllowAutoRedirect = false,
		PooledConnectionLifetime = TimeSpan.FromMinutes(2),
		PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
		MaxConnectionsPerServer = 1024
	});

builder.Services.AddHttpClient("ocelot")
	.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
	{
		PooledConnectionLifetime = TimeSpan.FromMinutes(2),
		PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
		MaxConnectionsPerServer = 1024
	});


builder.Services.AddCors(cors =>
{
	cors.AddDefaultPolicy(options =>
	{
		options.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials() // Важно для работы с куки
			.SetIsOriginAllowed(origin => true);
	});
});

WebApplication app = builder.Build();

app.Map("/", async context => { await context.Response.WriteAsync("available"); });

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseTokenValidation();

app.UseAuthorization();

app.MapControllers();

app.UseRouting();

app.UseWebSockets();

await app.UseOcelot();

app.Run();