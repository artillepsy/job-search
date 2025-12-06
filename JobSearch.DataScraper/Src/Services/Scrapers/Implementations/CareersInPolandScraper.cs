using JobSearch.DataScraper.Services.ConfigurationModels;
using JobSearch.DataScraper.Services.Options;
using JobSearch.DataScraper.Services.Result;

namespace JobSearch.DataScraper.Services.Scrapers.Implementations;

public class CareersInPolandScraper : ScraperBase
{
	private readonly HttpClient _httpClient;
	private CareersInPolandConfigModel _configModel;

	public CareersInPolandScraper(
		ILogger<CareersInPolandScraper> logger, 
		IHttpClientFactory httpClientFactory, 
		CareersInPolandConfigModel configModel) : base(logger, httpClientFactory)
	{
		_configModel = configModel;
		_httpClient = httpClientFactory.CreateClient();
		_logger.LogInformation($"[Constructor] config: {_configModel}");
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
			_logger.LogInformation($"pinging url: {_configModel.Url}");
			
			var response = await _httpClient.GetAsync(_configModel.Url, ct);
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