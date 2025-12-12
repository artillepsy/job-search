using JobSearch.DataScraper.Scraping.Configuration;
using JobSearch.DataScraper.Scraping.Configuration.Scrapers;
using JobSearch.DataScraper.Scraping.Scrapers.Implementations.CareersInPoland;

namespace JobSearch.DataScraper.Extensions;

public static class ServiceCollectionExtensions
{
	public static void BindConfigs(this IServiceCollection services)
	{
		services.AddOptions<ScraperServiceConfig>()
			.BindConfiguration("")
			.ValidateDataAnnotations()
			.ValidateOnStart();
		
		services.AddOptions<CareersInPolandConfig>()
			.BindConfiguration("")
			.ValidateDataAnnotations()
			.ValidateOnStart();
	}

	public static void RegisterConfigs(this IServiceCollection services)
	{
		services.AddScoped<ScraperServiceConfig>();
		services.AddScoped<CareersInPolandConfig>();
	}

	public static void RegisterScrapers(this IServiceCollection services)
	{
		services.AddScoped<CareersInPolandScraper>();
	}
}