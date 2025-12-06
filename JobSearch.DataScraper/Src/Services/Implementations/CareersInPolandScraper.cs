using JobSearch.DataScraper.Services.ConfigModels;
using JobSearch.DataScraper.Services.Core;
using JobSearch.DataScraper.Services.Core.Schedule;

namespace JobSearch.DataScraper.Services.Implementations;

public class CareersInPolandScraper : ScraperBase
{
	private readonly HttpClient _httpClient;
	private CareersInPolandConfig _config;

	public CareersInPolandScraper(
		ILogger<ScraperBase> logger, 
		IHttpClientFactory httpClientFactory, 
		CareersInPolandConfig config) : base(logger, httpClientFactory)
	{
		_config = config;
		_httpClient = httpClientFactory.CreateClient();
	}

	//todo: check proper configuration setup and try to check if http client works properly 
	public override async Task<ScrapingResult> ScrapeAsync(IScrapingOptions options, CancellationToken ct)
	{
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
			
			_logger.LogError(e, message: "CareersInPoland: scraping failed");
		}

		result.FinishedAt = DateTime.UtcNow;
		result.Duration = result.FinishedAt.Value - result.StartedAt;
		_logger.LogDebug( $"CareersInPoland: scraping finished. Result: {result}");

		return result;
	}
}