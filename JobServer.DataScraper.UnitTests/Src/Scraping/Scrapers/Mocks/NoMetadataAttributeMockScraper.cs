using JobSearch.DataScraper.Scraping;
using JobSearch.DataScraper.Scraping.Scrapers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JobServer.DataScraper.UnitTests.Scraping.Scrapers.Mocks;

public class NoMetadataAttributeMockScraper(
	ILogger<ScraperBase> logger,
	IHttpClientFactory httpClientFactory,
	IServiceScopeFactory scopeFactory)
	: ScraperBase(logger, httpClientFactory, scopeFactory)
{
	public override bool IsRunning() => false;

	public override Task<ScrapingResult> ScrapeAsync(CancellationToken ct)
	{
		return Task.FromResult(new ScrapingResult());	
	}
}