namespace JobSearch.DataScraper.Scraping.Scrapers;

public interface IScraper
{
	Task<ScrapingResult> ScrapeAsync(CancellationToken ct);
	bool IsRunning();
}