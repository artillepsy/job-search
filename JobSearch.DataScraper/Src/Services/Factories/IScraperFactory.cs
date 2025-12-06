using JobSearch.DataScraper.Services.Scrapers;

namespace JobSearch.DataScraper.Services.Factories;

public interface IScraperFactory
{
	public IScraper CreateScraper(string scraperName);
}