using System.Text.Json;
using System.Text.Json.Serialization;
using JobSearch.Data.Entities;
using JobSearch.DataScraper.Data.Repositories;
using JobSearch.DataScraper.Scraping.Attributes;
using JobSearch.DataScraper.Utils;

namespace JobSearch.DataScraper.Scraping.Scrapers.Implementations.UsaJobs;

[ScraperMetadata("UsaJobs")]
public class UsaJobsScraper : ScraperBase
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

	public UsaJobsScraper(
		ILogger<UsaJobsScraper> logger, 
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
			result.RecordsScraped = pageModels.Sum(m => m.SearchResult.SearchResultCount);
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

	private async Task CacheJobModelsAsync(IEnumerable<UsaJobsPageModel> pageModels)
	{
		var jobModels = new List<JobEntity>();

		foreach (var page in pageModels)
		foreach (var item in page.SearchResult.SearchResultItems)
		{
			var desc = item.MatchedObjectDescriptor;
			decimal? salaryMin = null, salaryMax = null;

			if (desc.PositionRemuneration.Count > 0)
			{
				salaryMin = decimal.Parse(desc.PositionRemuneration[0].MinimumRange);
				salaryMax = decimal.Parse(desc.PositionRemuneration[0].MaximumRange);
			}
			
			var jobModel = new JobEntity()
			{
				CompanyName = desc.OrganizationName,
				CreatedAt = desc.PublicationStartDate.ToUniversalTime(),
				Website = _config.Website,
				SalaryMin = salaryMin,
				SalaryMax = salaryMax,
				Currency = "USD",
				IsRemote = desc.RemoteIndicator,
				Location = desc.PositionLocationDisplay,
				Title = desc.PositionTitle,
				Url = desc.PositionURI,
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

	private async Task<List<UsaJobsPageModel>> GetAllPagesAsync(CancellationToken ct)
	{
		var pageModels = new List<UsaJobsPageModel>();
		var currPage = _config.PageStartIndex;

		while (!ct.IsCancellationRequested)
		{
			var pageModel = await GetPageAsync(currPage, ct);
			pageModels.Add(pageModel);

			_logger.LogInformation(
				$"[GetAllPagesAsync] page {currPage} " +
				$"of size {MemoryHelper.GetSerializedSize(pageModel)} B received. " +
				$"Total size: {MemoryHelper.GetSerializedSize(pageModels)} B");

			if (currPage == /*pageModel.SearchResult.UserArea.NumberOfPages*/ 3)
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

	private async Task<UsaJobsPageModel> GetPageAsync(int page, CancellationToken ct)
	{
		var url = $"{_config.BaseUrl}?Page={page}"; // todo: results per page 

		var response = await _httpClient.GetAsync(url, ct);
		response.EnsureSuccessStatusCode();

		var jsonString = await response.Content.ReadAsStringAsync(ct);

		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			NumberHandling = JsonNumberHandling.AllowReadingFromString // This would solve the "92108" vs int issue
		};
		
		var model = JsonSerializer.Deserialize<UsaJobsPageModel>(jsonString, options) ??
		            throw new InvalidOperationException();
		
		//_logger.LogInformation($"json model: {model}\nsize: {MemoryHelper.GetSerializedSize(model)}");

		return model;
	}

	#endregion
}