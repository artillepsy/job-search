using JobSearch.DataScraper.Scraping.Configuration.Scrapers;
using JobSearch.DataScraper.Scraping.Scrapers.Implementations.CareersInPoland;

namespace JobSearch.DataScraper.Utils;

public static class ScraperHelper
{
	public static readonly Dictionary<string, ScraperTypeBinding> TypeBindings = new()
	{
		["CareersInPoland"] = new ScraperTypeBinding()
		{
			Type = typeof(CareersInPolandScraper),
			ConfigType = typeof(CareersInPolandConfig),
		},
	};

	public class ScraperTypeBinding
	{
		public Type Type { get; set; }
		public Type ConfigType { get; set; }
	}
}