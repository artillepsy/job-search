using System.Net.Http.Headers;
using System.Reflection;
using JobSearch.DataScraper.Scraping.Attributes;
using JobSearch.DataScraper.Scraping.Scrapers;

namespace JobSearch.DataScraper.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddScrapersHttpClients(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		var email = configuration["Scrapers:Email"];
		if (email == null)
		{
			throw new ArgumentNullException(nameof(email));
		}
		
		AddDefaultHttpClient(services, "CareersInPoland", email);
		AddDefaultHttpClient(services, "Arbeitnow", email);

		services.AddHttpClient("UsaJobs", client =>
		{
			var apiKey = configuration["USAJobs:ApiKey"];
			
			client.BaseAddress = new Uri("https://data.usajobs.gov/api/");
			client.DefaultRequestHeaders.TryAddWithoutValidation("Host", "data.usajobs.gov");
			client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", email);
			client.DefaultRequestHeaders.Add("Authorization-Key", apiKey);
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		});
		return services;
	}

	private static void AddDefaultHttpClient(IServiceCollection services, string name, string email)
	{
		services.AddHttpClient(name, client =>
		{
			client.DefaultRequestHeaders.UserAgent.ParseAdd($"Mozilla/5.0 (compatible; JobScraper/1.0; +{email})");
			client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
			client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9,pl-PL;q=0.8");
			client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
			client.Timeout = TimeSpan.FromSeconds(30);
		});
	}

	public static void AddScrapers(this IServiceCollection services, IConfiguration configuration)
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