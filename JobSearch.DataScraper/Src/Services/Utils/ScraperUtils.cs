using JobSearch.DataScraper.Services.ConfigModels;
using JobSearch.DataScraper.Services.Implementations;

namespace JobSearch.DataScraper.Services.Utils;

public static class ScraperUtils
{
	public static readonly Dictionary<string, ScraperBinding> Bindings = new()
	{
			["CareersInPoland"] = new ScraperBinding()
			{
				Type = typeof(CareersInPolandScraper), 
				ConfigType = typeof(CareersInPolandConfig),
			},
	};

	public class ScraperBinding
	{
		public Type Type { get; set; }
		public Type ConfigType { get; set; }
	}
}
