using JobSearch.DataScraper.Data;
using JobSearch.DataScraper.Data.Repositories;
using JobSearch.DataScraper.Extensions;
using JobSearch.DataScraper.Scraping;
using JobSearch.DataScraper.Scraping.Scrapers.Factories;
using JobSearch.DataScraper.Scraping.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//db + ef setup
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddDbContext<AppDbContext>(opt =>
	opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
			o => o.MigrationsHistoryTable("__EFMigrationsHistory")) // Forces the default name
		.UseSnakeCaseNamingConvention());

// configs
builder.Configuration.AddConfigJsonFiles();
builder.Services.BindConfigs();
builder.Services.RegisterConfigs();

// scrapers
builder.Services.RegisterScrapers();

builder.Services.AddHttpClient("JobScraper", client =>
{
	client.DefaultRequestHeaders.UserAgent.ParseAdd(
		"Mozilla/5.0 (compatible; JobScraper/1.0; +artillepsy@gmail.com)"
	);
	client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
	client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
	client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
	client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("pl-PL,pl;q=0.8");
	client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
	client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddSingleton<ScrapingOptions>();
builder.Services.AddSingleton<IUrlHashService, UrlHashSha1Service>();
builder.Services.AddSingleton<IScraperFactory, ScraperFactory>();

// background service
// Register as concrete type AND as hosted service
builder.Services.AddSingleton<ScraperBackgroundService>();
builder.Services.AddHostedService(provider => 
	provider.GetRequiredService<ScraperBackgroundService>());
/*builder.Services.AddSingleton<ScraperBackgroundService>(provider => 
	provider.GetRequiredService<ScraperBackgroundService>());*/

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