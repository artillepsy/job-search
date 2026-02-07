using JobSearch.DataScraper.Scraping;
using JobSearch.DataScraper.Scraping.Attributes;
using JobSearch.DataScraper.Scraping.Scrapers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JobServer.DataScraper.UnitTests.Scraping.Scrapers.Mocks;

[ScraperMetadata("MockSection")]
public class MockScraper : ScraperBase
{
	private new bool _isRunning = false;
	public IConfiguration Section { get; }

	public MockScraper(
		ILogger<ScraperBase> logger, 
		IHttpClientFactory httpClientFactory, 
		IServiceScopeFactory scopeFactory,
		IConfiguration section) // Added this to match your AddScrapers logic
		: base(logger, httpClientFactory, scopeFactory)
	{
		Section = section;
	}

	public override bool IsRunning() => _isRunning;

	public override async Task<ScrapingResult> ScrapeAsync(CancellationToken ct)
	{
		_isRunning = true;
        
		await Task.Delay(10, ct); 
        
		_isRunning = false;

		return new ScrapingResult()
		{
			IsSuccess = true,
		};
	}
}