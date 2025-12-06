using JobSearch.DataScraper.Services.Options;
using JobSearch.DataScraper.Services.Result;

namespace JobSearch.DataScraper.Services.Scrapers;

public interface IScraper
{
	Task<ScrapingResult> ScrapeAsync(IScrapingOptions options, CancellationToken ct);
}