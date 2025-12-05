namespace JobSearch.DataScraper.Services.Core.Factory;

public interface IScraperFactory
{
	public IScraper CreateScraper(string scraperName);
}