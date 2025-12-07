using JobSearch.DataScraper.Database;
using JobSearch.DataScraper.Extensions;
using JobSearch.DataScraper.Services.Background;
using JobSearch.DataScraper.Services.ConfigurationModels;
using JobSearch.DataScraper.Services.Factories;
using JobSearch.DataScraper.Services.Options;
using JobSearch.DataScraper.Services.Utils;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//db + ef setup
builder.Services.AddDbContext<AppDbContext>(opt =>
	opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
		.UseSnakeCaseNamingConvention());

builder.Configuration.AddConfigJsonFiles();

builder.Services.AddOptions<ScraperServiceConfigModel>()
	.BindConfiguration("")
	.ValidateDataAnnotations()
	.ValidateOnStart();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IScrapingOptions, ScrapingOptions>();
builder.Services.AddSingleton<IScraperFactory, ScraperFactory>();

// background service
builder.Services.AddSingleton<IScraperBackgroundService, ScraperBackgroundService>();
builder.Services.AddHostedService<ScraperBackgroundService>();


foreach (var (name, binding) in ScraperUtils.Bindings)
{
	
	builder.Services.AddScoped(binding.Type);
	builder.Services.AddScoped(binding.ConfigType);
}

// mvc + swagger
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// misc
builder.Services.AddRouting(o => o.LowercaseUrls = true);

var app = builder.Build();

app.UseHttpsRedirection();

// swagger + openApi
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwaggerUI(o =>
	{
		o.SwaggerEndpoint("/openapi/v1.json", "API v1");
		o.RoutePrefix = "swagger"; // UI at /swagger
	});
}

// map api endpoints
app.MapControllers();

// Migrate DB (dev only or guarded)
if (app.Environment.IsDevelopment())
{
	using var scope = app.Services.CreateScope();
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	
	try
	{
		await db.Database.MigrateAsync();
	}
	catch (Exception ex)
	{
		app.Logger.LogError(ex, "Database migration failed");
	}
}	

app.Run();	