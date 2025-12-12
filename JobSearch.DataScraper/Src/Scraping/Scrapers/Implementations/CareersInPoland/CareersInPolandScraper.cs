using System.Text.Json;
using JobSearch.DataScraper.Data.Entities;
using JobSearch.DataScraper.Data.Repositories;
using JobSearch.DataScraper.Scraping.Configuration.Scrapers;
using JobSearch.DataScraper.Scraping.Scrapers.Implementations.CareersInPoland.Debug;
using JobSearch.DataScraper.Scraping.Services;
using JobSearch.DataScraper.Utils;
using Microsoft.Extensions.Options;

namespace JobSearch.DataScraper.Scraping.Scrapers.Implementations.CareersInPoland;

//todo: add chunks of data with higher load, but since there are tens of pages only, optimization can wait
public class CareersInPolandScraper : ScraperBase
{
	private readonly CareersInPolandConfig _config;

	public CareersInPolandScraper(
		ILogger<CareersInPolandScraper> logger, 
		IHttpClientFactory httpClientFactory, 
		IUrlHashService urlHashService,
		IOptions<CareersInPolandConfig> options, 
		IServiceScopeFactory scopeFactory) : base(logger, httpClientFactory, urlHashService, scopeFactory)
	{
		_config = options.Value;
		_logger.LogInformation($"[constructor] config: {_config}");
	}

	#region Scraping
	
	public override bool IsRunning() => _isRunning;

	//todo: check proper configuration setup and try to check if http client works properly 
	public override async Task<ScrapingResult> ScrapeAsync(ScrapingOptions options, CancellationToken ct)
	{
		_isRunning = true;
		var result = new ScrapingResult() { StartedAt = DateTime.UtcNow };

		try
		{
			var pageModels = await GetAllPagesAsync(ct);
			result.RecordsScraped = pageModels.Sum(m => m.JobOffers.Pagination.Data.Count);
			result.IsSuccess = true;
			await CacheJobModelsAsync(pageModels);
			
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

	#region Models Caching

	private async Task CacheJobModelsAsync(IEnumerable<CareersInPolandPageModel> pageModels)
	{
		var jobModels = new List<JobEntity>();

		foreach (var page in pageModels)
		foreach (var data in page.JobOffers.Pagination.Data)
		foreach (var location in data.Locations)
		{
			var jobModel = new JobEntity()
			{
				CompanyName = data.EmployerName,
				CreatedAt = data.Date.ToUniversalTime(),
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

		using var scope = _scopeFactory.CreateScope();
		var repository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
		
		await repository.AddUniqueAsync(jobModels);
		await repository.RemoveNonExistentAsync(jobModels);
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

			int randomDelay = RandomHelper.NextInt(_config.SearchIntervalMin, _config.SearchIntervalMax);
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