using System.Reflection;
using JobSearch.DataScraper.Scraping.Attributes;

namespace JobSearch.DataScraper.Scraping.Scrapers.Factories;

public class ScraperFactory : IScraperFactory
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly ILogger<ScraperFactory> _logger;
	private readonly Assembly _assembly;
	private readonly Dictionary<string, Type> _scraperTypes = new();
	
	public ScraperFactory(
		IServiceScopeFactory scopeFactory, 
		ILogger<ScraperFactory> logger,
		Assembly? assembly = null)
	{
		_scopeFactory = scopeFactory;
		_logger = logger;
		_assembly = assembly ?? Assembly.GetExecutingAssembly();
		_logger.LogInformation($"ScraperFactory");
		
		RegisterScrapers();
	}
	
	private void RegisterScrapers()
	{
		_logger.LogInformation($"RegisterScrapers");
		
		var types = _assembly.GetTypes()
			.Where(t => t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(typeof(ScraperBase)));

		foreach (var type in types)
		{
			var attribute = type.GetCustomAttribute<ScraperMetadataAttribute>();
            
			if (attribute != null)
			{
				_scraperTypes[attribute.SectionName.ToLowerInvariant()] = type;
				_logger.LogInformation($"Registered scraper '{attribute.SectionName}' from type {type.Name}");
			}
		}
	}

	public IScraper CreateScraper(string scraperName)
	{
		scraperName = scraperName.ToLowerInvariant();
		if (!_scraperTypes.TryGetValue(scraperName, out var scraperType))
		{
			throw new ArgumentException($"Scraper '{scraperName}' not found");
		}

		var scope = _scopeFactory.CreateScope();
		return (IScraper)scope.ServiceProvider.GetRequiredService(scraperType);
	}
}