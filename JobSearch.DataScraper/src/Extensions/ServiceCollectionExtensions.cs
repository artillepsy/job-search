using System.Reflection;
using JobSearch.DataScraper.Scraping.Attributes;
using JobSearch.DataScraper.Scraping.Scrapers;

namespace JobSearch.DataScraper.Extensions;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddScraperHttpClient()
		{
			services.AddHttpClient("JobScraper", client =>
			{
				client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; JobScraper/1.0; +artillepsy@gmail.com)");
				client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
				client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9,pl-PL;q=0.8");
				client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
				client.Timeout = TimeSpan.FromSeconds(30);
			});
			return services;
		}
		
		public void AddScrapers(IConfiguration configuration)
		{
			var assembly = typeof(ScraperBase).Assembly;
			var scraperTypes = assembly.GetTypes()
				.Where(t => t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(typeof(ScraperBase)));

			foreach (var type in scraperTypes)
			{
				var attr = type.GetCustomAttribute<ScraperMetadataAttribute>();
				if (attr == null) continue;

				// Automatically register as Scoped using a factory delegate
				services.AddScoped(type, sp => 
				{
					// Pull the specific section defined in the attribute
					var section = configuration.GetSection($"Scrapers:{attr.SectionName}");
            
					// Create the scraper, injecting the section manually
					return ActivatorUtilities.CreateInstance(sp, type, section);
				});
			}
		}
	}
}