namespace JobSearch.DataScraper.Services.Background;

public interface IScraperBackgroundService
{
	public Task StartScrapingAsync(string scraperName);
	
}