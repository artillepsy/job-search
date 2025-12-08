using System.Text.Json;
using JobSearch.DataScraper.Core.Random;
using JobSearch.DataScraper.Services.Configuration.Scrapers;
using JobSearch.DataScraper.Services.Options;
using JobSearch.DataScraper.Services.Result;
using JobSearch.DataScraper.Services.Scrapers.CareersInPoland.Debug;
using JobSearch.DataScraper.Services.Scrapers.CareersInPoland.Models;
using Microsoft.Extensions.Options;

namespace JobSearch.DataScraper.Services.Scrapers.CareersInPoland.Implementations;

public class CareersInPolandScraper : ScraperBase
{
	private readonly IRandomService _randomService;
	private readonly HttpClient _httpClient;
	private readonly CareersInPolandConfig _config;
	private bool _isRunning = false;

	public CareersInPolandScraper(
		IRandomService randomService,
		ILogger<CareersInPolandScraper> logger, 
		IHttpClientFactory httpClientFactory, 
		IOptions<CareersInPolandConfig> options) : base(logger, httpClientFactory)
	{
		_randomService = randomService;
		_config = options.Value;
		_httpClient = httpClientFactory.CreateClient();
		_logger.LogInformation($"[constructor] config: {_config}");
	}
	
	public override bool IsRunning() => _isRunning;

	//todo: check proper configuration setup and try to check if http client works properly 
	public override async Task<ScrapingResult> ScrapeAsync(IScrapingOptions options, CancellationToken ct)
	{
		_isRunning = true;
		var currPage = _config.PageStartIndex;
		
		var result = new ScrapingResult()
		{
			StartedAt = DateTime.UtcNow
		};
		
		while (!ct.IsCancellationRequested)
		{
			try
			{
				var pageModel = await GetPageAsync(currPage, ct);
				_logger.LogInformation($"page {currPage} received");

				result.RecordsScraped += pageModel.JobOffers.Pagination.Data.Count;
				
				if (pageModel.JobOffers.Pagination.IsLastPage)
				{
					break;
				}
			}
			catch (Exception e)
			{
				result.IsSuccess = false;
				result.Error = e.Message;
			
				_logger.LogError(e, message: "scraping failed");
			}

			currPage++;
				
			int randomDelay = _randomService.NextInt(_config.SearchIntervalMin, _config.SearchIntervalMax);
			_logger.LogInformation($"waiting for  {randomDelay} millisec");

			await Task.Delay(randomDelay, ct);
		}

		result.IsSuccess = true;
		result.FinishedAt = DateTime.UtcNow;
		result.Duration = result.FinishedAt.Value - result.StartedAt;
		
		_logger.LogInformation( $"scraping finished. Result: {result}");

		_isRunning = false;
		return result;
	}

	private async Task<CareersInPolandPageModel> GetPageAsync(int page, CancellationToken ct)
	{
		var url = $"{_config.BaseUrl}{page}";
		CareersInPolandPageModel model;
		
		try
		{
			/*var response = await _httpClient.GetAsync(url, ct);
			response.EnsureSuccessStatusCode();
			
			var jsonString = await response.Content.ReadAsStringAsync(ct);*/

			var jsonString = CareersInPolandDebug.GetMockJsonPage(page);
			model = JsonSerializer.Deserialize<CareersInPolandPageModel>(jsonString) ?? throw new InvalidOperationException();
			
			_logger.LogInformation($"json model: {model}");
		}
		catch (Exception e)
		{
			_logger.LogError(e, "page retrieving failed");
			throw;
		}

		return model;
	}
}