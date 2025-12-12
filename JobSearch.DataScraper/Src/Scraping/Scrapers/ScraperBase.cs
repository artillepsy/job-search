using JobSearch.DataScraper.Scraping.Services;

namespace JobSearch.DataScraper.Scraping.Scrapers;

public abstract class ScraperBase : IScraper
{
	protected readonly IServiceScopeFactory _scopeFactory;
	protected readonly ILogger<ScraperBase> _logger;
	protected readonly IUrlHashService _urlHashService;
	protected readonly IHttpClientFactory _httpClientFactory;
	protected readonly HttpClient _httpClient;
	protected bool _isRunning = false;

	protected ScraperBase(
		ILogger<ScraperBase> logger, 
		IHttpClientFactory httpClientFactory, 
		IUrlHashService urlHashService, 
		IServiceScopeFactory scopeFactory)
	{
		_logger = logger;
		_httpClientFactory = httpClientFactory;
		_urlHashService = urlHashService;
		_scopeFactory = scopeFactory;
		_httpClient = _httpClient = httpClientFactory.CreateClient();
	}

	public abstract bool IsRunning();
	public abstract Task<ScrapingResult> ScrapeAsync(ScrapingOptions options, CancellationToken ct);
}