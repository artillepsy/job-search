using JobSearch.DataScraper.Scraping.Services;

namespace JobSearch.DataScraper.Scraping.Scrapers;

/// <summary>
/// Base class for scrapers. In order to setup a new scraper.
/// </summary>
public abstract class ScraperBase : IScraper
{
	protected readonly IServiceScopeFactory _scopeFactory;
	protected readonly ILogger<ScraperBase> _logger;
	protected readonly IHttpClientFactory _httpClientFactory;
	protected bool _isRunning = false;

	protected ScraperBase(
		ILogger<ScraperBase> logger, 
		IHttpClientFactory httpClientFactory, 
		IServiceScopeFactory scopeFactory)
	{
		_logger = logger;
		_httpClientFactory = httpClientFactory;
		_scopeFactory = scopeFactory;
	}

	public abstract bool IsRunning();
	public abstract Task<ScrapingResult> ScrapeAsync(CancellationToken ct);
}