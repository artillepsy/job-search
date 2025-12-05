using JobSearch.DataScraper.Services.Core.Schedule;

namespace JobSearch.DataScraper.Services.Core;

public interface IScraper
{
	Task<ScrapingResult> ScrapeAsync(IScrapingOptions options, CancellationToken ct);
}