using System.Text.Json;
using JobSearch.Data.Entities;
using JobSearch.DataScraper.Data.Repositories;
using JobSearch.DataScraper.Scraping.Attributes;
using JobSearch.DataScraper.Utils;

namespace JobSearch.DataScraper.Scraping.Scrapers.Implementations.Arbeitnow;

[ScraperMetadata("Arbeitnow")]
public class ArbeitnowScraper : ScraperBase
{
	private readonly HttpClient _httpClient;
	private readonly Config _config;

	private class Config 
	{
		public string BaseUrl { get; set; } = string.Empty;
		public string Website { get; set; } = string.Empty;
		public string HttpClientName { get; set; } = string.Empty;
		
		public int PageStartIndex { get; set; }
		public int SearchIntervalMin { get; set; }
		public int SearchIntervalMax { get; set; }
	}

	public ArbeitnowScraper(
		ILogger<ArbeitnowScraper> logger, 
		IHttpClientFactory httpClientFactory, 
		IConfiguration section, 
		IServiceScopeFactory scopeFactory) : base(logger, httpClientFactory, scopeFactory)
	{
		_config = new Config();
		section.Bind(_config);
		_httpClient = _httpClient = httpClientFactory.CreateClient(_config.HttpClientName);
	}

	#region Scraping
	
	public override bool IsRunning() => _isRunning;

	//todo: check proper configuration setup and try to check if http client works properly 
	public override async Task<ScrapingResult> ScrapeAsync(CancellationToken ct)
	{
		_isRunning = true;
		var result = new ScrapingResult() { StartedAt = DateTime.UtcNow };

		try
		{
			var pageModels = await GetAllPagesAsync(ct);
			result.RecordsScraped = pageModels.Sum(m => m.Data.Count);
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

	private async Task CacheJobModelsAsync(IEnumerable<ArbeitnowPageModel> pageModels)
	{
		var jobModels = new List<JobEntity>();

		foreach (var page in pageModels)
		foreach (var item in page.Data)
		{
			var jobModel = new JobEntity()
			{
				CompanyName = item.CompanyName,
				CreatedAt = item.CreatedAt,
				Website = _config.Website,
				SalaryMin = null, // todo: find out how to get salary data
				SalaryMax = null,
				Currency = null,
				IsRemote = item.IsRemote,
				Location = item.Location,
				Title = item.Title,
				Url = item.JobUrl,
			};
			jobModels.Add(jobModel);
		}
		
		_logger.LogInformation(
			$"items count: {jobModels.Count}\n\n" +
		           $"{string.Join("\n", jobModels.Select(j => $"{j.Title}; {j.CreatedAt}; {j.CompanyName}; {j.Url}"))}");

		using var scope = _scopeFactory.CreateScope();
		var repository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
		
		await repository.AddUniqueAsync(jobModels);
		await repository.RemoveNonExistentAsync(jobModels);
	}

	#endregion
	
	#region Pagination

	private async Task<List<ArbeitnowPageModel>> GetAllPagesAsync(CancellationToken ct)
	{
		var pageModels = new List<ArbeitnowPageModel>();
		var currPage = _config.PageStartIndex;

		while (!ct.IsCancellationRequested)
		{
			var pageModel = await GetPageAsync(currPage, ct);
			pageModels.Add(pageModel);

			_logger.LogInformation(
				$"[GetAllPagesAsync] page {currPage} " +
				$"of size {MemoryHelper.GetSerializedSize(pageModel)} B received. " +
				$"Total size: {MemoryHelper.GetSerializedSize(pageModels)} B");

			if (currPage == 3 /*string.IsNullOrEmpty(pageModel.NextPageLink)*/) // todo: change 
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

	private async Task<ArbeitnowPageModel> GetPageAsync(int page, CancellationToken ct)
	{
		var url = $"{_config.BaseUrl}{page}"; // todo: results per page 

		var response = await _httpClient.GetAsync(url, ct);
		response.EnsureSuccessStatusCode();

		var jsonString = await response.Content.ReadAsStringAsync(ct);

		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
		};
		
		var model = JsonSerializer.Deserialize<ArbeitnowPageModel>(jsonString, options) ??
		            throw new InvalidOperationException();
		
		//_logger.LogInformation($"json model: {model}\nsize: {MemoryHelper.GetSerializedSize(model)}");

		return model;
	}

	#endregion
}