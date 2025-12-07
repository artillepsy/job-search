using JobSearch.DataScraper.Services.Configuration.Scrapers;
using JobSearch.DataScraper.Services.Options;
using JobSearch.DataScraper.Services.Result;
using Microsoft.Extensions.Options;

namespace JobSearch.DataScraper.Services.Scrapers.Implementations;

public class CareersInPolandScraper : ScraperBase
{
	private readonly HttpClient _httpClient;
	private readonly CareersInPolandConfig _config;
	private bool _isRunning = false;

	public CareersInPolandScraper(
		ILogger<CareersInPolandScraper> logger, 
		IHttpClientFactory httpClientFactory, 
		IOptions<CareersInPolandConfig> options) : base(logger, httpClientFactory)
	{
		_config = options.Value;
		_httpClient = httpClientFactory.CreateClient();
		_logger.LogInformation($"[Constructor] config: {_config}");
	}

	//todo: check proper configuration setup and try to check if http client works properly 
	public override async Task<ScrapingResult> ScrapeAsync(IScrapingOptions options, CancellationToken ct)
	{
		_isRunning = true;
		
		var result = new ScrapingResult()
		{
			StartedAt = DateTime.UtcNow
		};

		try
		{
			_logger.LogInformation($"pinging url: {_config.Url}");
			
			var response = await _httpClient.GetAsync(_config.Url, ct);
			response.EnsureSuccessStatusCode();

			var content = await response.Content.ReadAsStringAsync(ct);

			_logger.LogInformation($"response content: {content}");
			
			result.IsSuccess = true;
			result.RecordsScraped = 0;
		}
		catch (Exception e)
		{
			result.IsSuccess = false;
			result.Error = e.Message;
			
			_logger.LogError(e, message: "scraping failed");
		}

		result.FinishedAt = DateTime.UtcNow;
		result.Duration = result.FinishedAt.Value - result.StartedAt;
		_logger.LogDebug( $"scraping finished. Result: {result}");

		_isRunning = false;
		return result;
	}

	public override bool IsRunning() => _isRunning;
}