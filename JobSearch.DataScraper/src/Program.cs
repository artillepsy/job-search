using JobSearch.Data;
using JobSearch.DataScraper.Data.Repositories;
using JobSearch.DataScraper.Extensions;
using JobSearch.DataScraper.Scraping.Scrapers.Factories;
using JobSearch.DataScraper.Scraping.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Extract the scraper name from args
string? scraperToRun = args.SkipWhile(a => a != "--scraper").Skip(1).FirstOrDefault();

// DB & Repositories
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddDbContext<AppDbContext>(opt =>
	opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
			o => o.MigrationsHistoryTable("__EFMigrationsHistory")) // Forces the default name
		.UseSnakeCaseNamingConvention());

builder.Services.AddScrapers(builder.Configuration);
builder.Services.AddScrapersHttpClients(builder.Configuration);
builder.Services.AddSingleton<IUrlHashService, UrlHashSha1Service>();
builder.Services.AddSingleton<IScraperFactory, ScraperFactory>();

using IHost host = builder.Build();

using var scope = host.Services.CreateScope();

var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
var factory = scope.ServiceProvider.GetRequiredService<IScraperFactory>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

try
{
	logger.LogInformation("Migrating database...");
	await db.Database.MigrateAsync();
	logger.LogInformation("Migration successful.");

	logger.LogInformation("Starting scheduled job execution...");
	if (!string.IsNullOrEmpty(scraperToRun))
	{
		logger.LogInformation($"Running scraper: {scraperToRun}");
		
		var scraper = factory.CreateScraper(scraperToRun.ToLowerInvariant());
		await scraper.ScrapeAsync(CancellationToken.None);
	}
	logger.LogInformation("Job finished successfully.");
	Environment.Exit(0);
}
catch (Exception ex)
{
	logger.LogCritical(ex, "Job execution failed.");
	Environment.Exit(1);
}