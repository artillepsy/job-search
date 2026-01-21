using System.Text.Json;
using System.Text.RegularExpressions;
using JobSearch.Data.Entities;
using JobSearch.DataScraper.Data.Repositories;
using JobSearch.DataScraper.Scraping.Attributes;
using JobSearch.DataScraper.Scraping.Services;
using JobSearch.DataScraper.Utils;

namespace JobSearch.DataScraper.Scraping.Scrapers.Implementations.CareersInPoland;

//todo: add chunks of data with higher load, but since there are tens of pages only, optimization can wait
[ScraperMetadata("CareersInPoland")]
public class CareersInPolandScraper : ScraperBase
{
	private readonly HttpClient _httpClient;
	private readonly Config _config;

	private class Config 
	{
		public bool Enabled { get; set; }
		
		public string BaseUrl { get; set; } = string.Empty;
		public string Website { get; set; } = string.Empty;
		public string HttpClientName { get; set; } = string.Empty;
		
		public int PageStartIndex { get; set; }
		public int SearchIntervalMin { get; set; }
		public int SearchIntervalMax { get; set; }
	}

	public CareersInPolandScraper(
		ILogger<CareersInPolandScraper> logger, 
		IHttpClientFactory httpClientFactory, 
		IUrlHashService urlHashService,
		IConfiguration section, 
		IServiceScopeFactory scopeFactory) : base(logger, httpClientFactory, urlHashService, scopeFactory)
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
			var salary = ParseSalary(data.Salary);
			
			var jobModel = new JobEntity()
			{
				CompanyName = data.EmployerName,
				CreatedAt = data.Date.ToUniversalTime(),
				Website = _config.Website,
				SalaryMin = salary.min,
				SalaryMax = salary.max,
				Currency = salary.currency,
				Location = location.Location,
				Title = data.Title,
				Url = location.FullUrl,
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

	private (decimal? min, decimal? max, string? currency) ParseSalary(string? salaryRaw)
	{
		if (string.IsNullOrWhiteSpace(salaryRaw))
		{
			return (null, null, null);
		}

		// Pattern: Number - [Optional Number] [Rest of string]
		var match = Regex.Match(salaryRaw, @"(\d+)\s*-\s*(\d+)?\s*(.*)", RegexOptions.IgnoreCase);

		if (!match.Success) return (null, null, null);

		decimal? min = decimal.TryParse(match.Groups[1].Value, out var minVal) ? minVal : null;
		decimal? max = decimal.TryParse(match.Groups[2].Value, out var maxVal) ? maxVal : null;
		string currency = match.Groups[3].Value.Trim().ToUpper();

		return (min, max, currency);
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

		var response = await _httpClient.GetAsync(url, ct);
		response.EnsureSuccessStatusCode();

		var jsonString = await response.Content.ReadAsStringAsync(ct);

		var model = JsonSerializer.Deserialize<CareersInPolandPageModel>(jsonString) ??
		            throw new InvalidOperationException();
		
		//_logger.LogInformation($"json model: {model}\nsize: {MemoryHelper.GetSerializedSize(model)}");

		return model;
	}

	#endregion
}