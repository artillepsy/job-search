using JobSearch.DataScraper.Core.Urls;
using JobSearch.DataScraper.Services.Options;
using JobSearch.DataScraper.Services.Result;

namespace JobSearch.DataScraper.Services.Scrapers;

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
	public abstract Task<ScrapingResult> ScrapeAsync(IScrapingOptions options, CancellationToken ct);
}