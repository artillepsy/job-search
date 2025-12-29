using System.Collections.Concurrent;
using System.Threading.Channels;
using JobSearch.DataScraper.Scraping.Scrapers;
using JobSearch.DataScraper.Scraping.Scrapers.Factories;

namespace JobSearch.DataScraper.Scraping;

public class ScraperBackgroundService : BackgroundService
{
	private readonly ILogger<ScraperBackgroundService> _logger;
	private readonly IScraperFactory _scraperFactory;
	private readonly ScrapingOptions _scrapingOptions;
	private int _simpleCounter = 0;

	// queue of commands?
	private readonly Channel<string> _commands;

	private readonly ConcurrentDictionary<string, IScraper> _registeredScrapers = new();

	public ScraperBackgroundService(
		ILogger<ScraperBackgroundService> logger,
		IScraperFactory scraperFactory,
		ScrapingOptions scrapingOptions)
	{
		_logger = logger;
		_scraperFactory = scraperFactory;
		_scrapingOptions = scrapingOptions;
		_commands = Channel.CreateUnbounded<string>();
	}
	// enqueue execution command?
	
	// if i have only one instance of a background service, every time i want to access it,
	// i should check if it's busy or not
	public async Task StartScrapingAsync(string scraperName)
	{
		if (!_registeredScrapers.ContainsKey(scraperName))
		{
			_registeredScrapers.TryAdd(scraperName, _scraperFactory.CreateScraper(scraperName));
		}
		
		await _commands.Writer.WriteAsync(scraperName);
		_logger.LogInformation($"[StartScrapingAsync] Thread: {Thread.CurrentThread.ManagedThreadId}, dict. items count: {_registeredScrapers.Count}, channel items: {_commands.Reader.Count}");
		_simpleCounter++;
	}

	protected override async Task ExecuteAsync(CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			var cmd = await _commands.Reader.ReadAsync(ct);
			
			_logger.LogInformation($"[ExecuteAsync] Thread: {Thread.CurrentThread.ManagedThreadId} Try run command [{cmd}]");

			var scraper = _registeredScrapers[cmd];
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
				_logger.LogError(e, $"command execution failed for {cmd}");
				throw;
			}
		}
	}
}