namespace JobSearch.DataScraper.Services.ConfigurationModels;

public class ScraperServiceConfigModel
{
	public List<AllowedScraper> AllowedScrapers { get; set; }

	public class AllowedScraper
	{
		public string Name { get; set; }
		public bool IsEnabled { get; set; }
		public string ConfigPath { get; set; }
	}
}