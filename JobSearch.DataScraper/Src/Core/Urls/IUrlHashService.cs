namespace JobSearch.DataScraper.Core.Urls;

public interface IUrlHashService
{
	public string HashUrl(string id, string url);
}