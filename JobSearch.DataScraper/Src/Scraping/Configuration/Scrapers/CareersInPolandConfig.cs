namespace JobSearch.DataScraper.Scraping.Configuration.Scrapers;

public class CareersInPolandConfig
{
	public string Website { get; set; } = "";
	public string BaseUrl { get; set; } = "";
	public int PageStartIndex { get; set; }
	public int SearchIntervalMin { get; set; } // millisec
	public int SearchIntervalMax { get; set; } // millisec
	public int MaxRetries { get; set; }

	public override string ToString() =>
		$"Website: {Website}\n" +
		$"BaseUrl: {BaseUrl}\n" +
		$"PageStartIndex: {PageStartIndex}\n" +
		$"SearchIntervalMin: {SearchIntervalMin}\n" +
		$"SearchIntervalMin: {SearchIntervalMax}\n" +
		$"MaxRetries: {MaxRetries}\n";
}