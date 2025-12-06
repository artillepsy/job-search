using JobSearch.DataScraper.Services.Core.Factory;

namespace JobSearch.DataScraper.Services.Core.Schedule;

public class ScraperBackgroundService : BackgroundService
{
	private readonly ILogger<ScraperBackgroundService> _logger;
	private readonly IScraperFactory _scraperFactory;
	private string _currScraperName;

	public ScraperBackgroundService(ILogger<ScraperBackgroundService> logger, IScraperFactory scraperFactory)
	{
		_logger = logger;
		_scraperFactory = scraperFactory;
	}

	public void StartScraping(string scraperName)
	{
		_currScraperName = scraperName;
		_logger.LogInformation("Scraping started");
		
	}
	
	protected override async Task ExecuteAsync(CancellationToken ct)
	{
		_logger.LogInformation("Scraper Background Service started");

		while (!ct.IsCancellationRequested)
		{
			try
			{
				
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}