namespace JobSearch.DataScraper.Scraping.Scrapers;

public interface IScraper
{
	Task<ScrapingResult> ScrapeAsync(ScrapingOptions options, CancellationToken ct);
	bool IsRunning();
}