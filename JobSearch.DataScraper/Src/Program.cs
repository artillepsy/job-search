using JobSearch.DataScraper.Database;
using JobSearch.DataScraper.Extensions;
using JobSearch.DataScraper.Services.Background;
using JobSearch.DataScraper.Services.Factories;
using JobSearch.DataScraper.Services.Options;
using JobSearch.DataScraper.Services.Random;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//db + ef setup
builder.Services.AddDbContext<AppDbContext>(opt =>
	opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
		.UseSnakeCaseNamingConvention());

// configs
builder.Configuration.AddConfigJsonFiles();
builder.Services.BindConfigs();
builder.Services.RegisterConfigs();

// scrapers
builder.Services.RegisterScrapers();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IScrapingOptions, ScrapingOptions>();
builder.Services.AddSingleton<IRandomService, RandomService>();
builder.Services.AddSingleton<IScraperFactory, ScraperFactory>();

// background service
// Register as concrete type AND as hosted service
builder.Services.AddSingleton<ScraperBackgroundService>();
builder.Services.AddHostedService(provider => 
	provider.GetRequiredService<ScraperBackgroundService>());
builder.Services.AddSingleton<IScraperBackgroundService>(provider => 
	provider.GetRequiredService<ScraperBackgroundService>());

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