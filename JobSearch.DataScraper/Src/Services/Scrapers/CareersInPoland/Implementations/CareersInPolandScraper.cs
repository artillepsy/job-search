using System.Text.Json;
using JobSearch.DataScraper.Core.Random;
using JobSearch.DataScraper.Core.Urls;
using JobSearch.DataScraper.Core.Utils;
using JobSearch.DataScraper.Database.Models;
using JobSearch.DataScraper.Database.Repositories;
using JobSearch.DataScraper.Services.Configuration.Scrapers;
using JobSearch.DataScraper.Services.Options;
using JobSearch.DataScraper.Services.Result;
using JobSearch.DataScraper.Services.Scrapers.CareersInPoland.Debug;
using JobSearch.DataScraper.Services.Scrapers.CareersInPoland.Models;
using Microsoft.Extensions.Options;

namespace JobSearch.DataScraper.Services.Scrapers.CareersInPoland.Implementations;

//todo: add chunks of data with higher load, but since there are tens of pages only, optimization can wait
public class CareersInPolandScraper : ScraperBase
{
	private readonly IRandomService _randomService;
	private readonly CareersInPolandConfig _config;

	public CareersInPolandScraper(
		ILogger<CareersInPolandScraper> logger, 
		IHttpClientFactory httpClientFactory, 
		IUrlHashService urlHashService,
		IJobRepository repository,
		IRandomService randomService,
		IOptions<CareersInPolandConfig> options) : base(logger, httpClientFactory, urlHashService, repository)
	{
		_randomService = randomService;
		_config = options.Value;
		_logger.LogInformation($"[constructor] config: {_config}");
	}

	#region Scraping
	
	public override bool IsRunning() => _isRunning;

	//todo: check proper configuration setup and try to check if http client works properly 
	public override async Task<ScrapingResult> ScrapeAsync(IScrapingOptions options, CancellationToken ct)
	{
		_isRunning = true;
		var result = new ScrapingResult() { StartedAt = DateTime.UtcNow };

		try
		{
			var pageModels = await GetAllPagesAsync(ct);
			result.RecordsScraped = pageModels.Sum(m => m.JobOffers.Pagination.Data.Count);
			result.IsSuccess = true;
			
			
			_logger.LogInformation( $"Scraping finished");
		}
		catch (Exception e)
		{
			result.IsSuccess = false;
			result.Error = e.Message;

			_logger.LogError(e, message: "Scraping failed");
		}

		result.FinishedAt = DateTime.UtcNow;
		result.Duration = result.FinishedAt.Value - result.StartedAt;
		
		_logger.LogInformation( $"Scraping result: {result}");

		_isRunning = false;
		return result;
	}

	#endregion

	#region Models

	private async Task CacheJobModels(IEnumerable<CareersInPolandPageModel> pageModels)
	{
		var jobModels = new List<JobModel>();

		foreach (var page in pageModels)
		foreach (var data in page.JobOffers.Pagination.Data)
		foreach (var location in data.Locations)
		{
			var jobModel = new JobModel()
			{
				CompanyName = data.EmployerName,
				CreatedAt = data.Date,
				Website = _config.Website,
				WebsiteSpecificId = data.Id,
				IsSalaryVisible = !string.IsNullOrEmpty(data.Salary),
				Salary = data.Salary,
				Location = location.Location,
				Title = data.Title,
				Url = location.FullUrl,
				Sha1UrlHash = _urlHashService.HashUrl(data.Id, location.FullUrl),
			};
			jobModels.Add(jobModel);
		}

		await _repository.AddUniqueAsync(jobModels);
	}

	#endregion
	
	#region Pagination

	private async Task<List<CareersInPolandPageModel>> GetAllPagesAsync(CancellationToken ct)
	{
		var pageModels = new List<CareersInPolandPageModel>();
		var currPage = _config.PageStartIndex;

		while (!ct.IsCancellationRequested)
		{
			var pageModel = await GetPageAsync(currPage, ct);
			pageModels.Add(pageModel);

			_logger.LogInformation(
				$"[GetAllPagesAsync] page {currPage} " +
				$"of size {MemoryHelper.GetSerializedSize(pageModel)} B received. " +
				$"Total size: {MemoryHelper.GetSerializedSize(pageModels)} B");

			if (pageModel.JobOffers.Pagination.IsLastPage)
			{
				break;
			}

			currPage++;

			int randomDelay = _randomService.NextInt(_config.SearchIntervalMin, _config.SearchIntervalMax);
			_logger.LogInformation($"waiting for  {randomDelay} millisec");

			await Task.Delay(randomDelay, ct);
		}
		
		return pageModels;
	}

	private async Task<CareersInPolandPageModel> GetPageAsync(int page, CancellationToken ct)
	{
		var url = $"{_config.BaseUrl}{page}";
		CareersInPolandPageModel model;

		/*var response = await _httpClient.GetAsync(url, ct);
		response.EnsureSuccessStatusCode();

		var jsonString = await response.Content.ReadAsStringAsync(ct);*/

		var jsonString = CareersInPolandDebug.GetMockJsonPage(page);
		model = JsonSerializer.Deserialize<CareersInPolandPageModel>(jsonString) ??
		        throw new InvalidOperationException();

		//_logger.LogInformation($"json model: {model}\nsize: {MemoryHelper.GetSerializedSize(model)}");

		return model;
	}

	#endregion
}