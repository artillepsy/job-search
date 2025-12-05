using JobSearch.DataScraper.Services.Core.Schedule;

namespace JobSearch.DataScraper.Services.Core;

public abstract class ScraperBase : IScraper
{
	protected readonly ILogger<ScraperBase> _logger;
	protected readonly IHttpClientFactory _httpClientFactory;
	
	public ScraperBase(ILogger<ScraperBase> logger, IHttpClientFactory httpClientFactory)
	{
		_logger = logger;
		_httpClientFactory = httpClientFactory;
	}

	public abstract Task<ScrapingResult> ScrapeAsync(IScrapingOptions options, CancellationToken ct);
}