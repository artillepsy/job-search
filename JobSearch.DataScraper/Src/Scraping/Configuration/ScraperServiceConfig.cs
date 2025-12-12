namespace JobSearch.DataScraper.Scraping.Configuration;

public class ScraperServiceConfig
{
	public List<ScraperConfig> AllowedScrapers { get; set; }

	public class ScraperConfig
	{
		public string Name { get; set; }
		public bool IsEnabled { get; set; }
		public string ConfigPath { get; set; }
	}

	public override string ToString()
	{
		return "allowed scrapers:\n\n" +
		       string.Join(" \n ", AllowedScrapers.Select(s => $"Name: {s.Name}, IsEnabled: {s.IsEnabled}, ConfigPath: {s.ConfigPath}"));
	}
}