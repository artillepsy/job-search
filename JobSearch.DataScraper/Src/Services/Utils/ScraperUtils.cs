using JobSearch.DataScraper.Services.ConfigurationModels;
using JobSearch.DataScraper.Services.ConfigurationModels.Scrapers;
using JobSearch.DataScraper.Services.Scrapers.Implementations;

namespace JobSearch.DataScraper.Services.Utils;

public static class ScraperUtils
{
	public static readonly Dictionary<string, ScraperBinding> Bindings = new()
	{
		["CareersInPoland"] = new ScraperBinding()
		{
			Type = typeof(CareersInPolandScraper),
			ConfigType = typeof(CareersInPolandConfigModel),
		},
	};

	public class ScraperBinding
	{
		public Type Type { get; set; }
		public Type ConfigType { get; set; }
	}
}