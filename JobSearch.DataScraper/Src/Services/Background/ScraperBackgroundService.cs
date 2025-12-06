using JobSearch.DataScraper.Services.Factories;
using JobSearch.DataScraper.Services.Options;

namespace JobSearch.DataScraper.Services.Background;

public class ScraperBackgroundService : BackgroundService, IScraperBackgroundService
{
	private readonly ILogger<ScraperBackgroundService> _logger;
	private readonly IScraperFactory _scraperFactory;
	private readonly IScrapingOptions _scrapingOptions;

	private string _currScraperName = "";

	public ScraperBackgroundService(
		ILogger<ScraperBackgroundService> logger,
		IScraperFactory scraperFactory,
		IScrapingOptions scrapingOptions)
	{
		_logger = logger;
		_scraperFactory = scraperFactory;
		_scrapingOptions = scrapingOptions;
	}

	public async Task StartScrapingAsync(string scraperName)
	{
		_currScraperName = scraperName;
		_logger.LogInformation("Scraping started");
		await ExecuteAsync(new CancellationToken());
	}

	protected override async Task ExecuteAsync(CancellationToken ct)
	{
		_logger.LogInformation("Scraper Background Service started");

		while (!ct.IsCancellationRequested)
		{
			try
			{
				var scraper = _scraperFactory.CreateScraper(_currScraperName);
				await scraper.ScrapeAsync(_scrapingOptions, ct);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}