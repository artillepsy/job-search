namespace JobSearch.DataScraper.Scraping.Services;

public interface IUrlHashService
{
	public string HashUrl(string id, string url);
}