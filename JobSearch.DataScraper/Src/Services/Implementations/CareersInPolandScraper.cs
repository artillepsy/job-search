using JobSearch.DataScraper.Services.ConfigModels;
using JobSearch.DataScraper.Services.Core;
using JobSearch.DataScraper.Services.Core.Schedule;

namespace JobSearch.DataScraper.Services.Implementations;

public class CareersInPolandScraper : ScraperBase
{
	private CareersInPolandConfig _config;

	public CareersInPolandScraper(
		ILogger<ScraperBase> logger, 
		IHttpClientFactory httpClientFactory, 
		CareersInPolandConfig config) : base(logger, httpClientFactory)
	{
		_config = config;
	}

	public override async Task<ScrapingResult> ScrapeAsync(IScrapingOptions options, CancellationToken ct)
	{
		var result = new ScrapingResult()
		{
			StartedAt = DateTime.UtcNow
		};

		try
		{
			await Task.Delay(1000, ct); // Simulate work
            
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