using JobSearch.DataScraper.Services.Configuration;
using JobSearch.DataScraper.Services.Configuration.Scrapers;
using JobSearch.DataScraper.Services.Scrapers.Implementations;

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