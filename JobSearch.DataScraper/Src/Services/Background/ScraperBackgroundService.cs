using System.Collections.Concurrent;
using JobSearch.DataScraper.Services.Factories;
using JobSearch.DataScraper.Services.Options;
using JobSearch.DataScraper.Services.Scrapers;

namespace JobSearch.DataScraper.Services.Background;

public class ScraperBackgroundService : BackgroundService, IScraperBackgroundService
{
	private readonly ILogger<ScraperBackgroundService> _logger;
	private readonly IScraperFactory _scraperFactory;
	private readonly IScrapingOptions _scrapingOptions;

	// queue of commands?
	private ConcurrentQueue<string> _commands = new();

	private Dictionary<string, IScraper> _registeredScrapers = new();

	public ScraperBackgroundService(
		ILogger<ScraperBackgroundService> logger,
		IScraperFactory scraperFactory,
		IScrapingOptions scrapingOptions)
	{
		_logger = logger;
		_scraperFactory = scraperFactory;
		_scrapingOptions = scrapingOptions;
	}

	// enqueue execution command?
	
	// if i have only one instance of a background service, every time i want to access it,
	// i should check if it's busy or not
	public async Task StartScrapingAsync(string scraperName)
	{
		_registeredScrapers.TryAdd(scraperName, _scraperFactory.CreateScraper(scraperName));
		
		_commands.Enqueue(scraperName);
		_logger.LogInformation($"Scraping started [mock] for {scraperName}");
	}

	protected override async Task ExecuteAsync(CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			while (_commands.TryDequeue(out var scraperName))
			{
				var scraper = _registeredScrapers[scraperName];
				if (scraper.IsRunning())
				{
					continue;
				}
					
				try
				{
					// or just forget | store exec info in a variable
					await scraper.ScrapeAsync(_scrapingOptions, ct);
				}
				catch (Exception e)
				{
					_logger.LogError(e, $"command execution failed for {scraperName}");
					throw;
				}
				
			}
			
			
		}
	}
}