namespace JobSearch.DataScraper.Scraping.Scrapers.Factories;

public interface IScraperFactory
{
	public IScraper CreateScraper(string scraperName);
}