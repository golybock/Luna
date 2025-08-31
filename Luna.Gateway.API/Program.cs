using Luna.Gateway.API.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration).AddDelegatingHandler<CookieForwardingDelegatingHandler>(true);
;

builder.Services.AddHttpClient("AuthValidation")
	.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
	{
		UseCookies = true,
		CookieContainer = new System.Net.CookieContainer(),
		AllowAutoRedirect = false
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

// Configure the HTTP request pipeline.
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