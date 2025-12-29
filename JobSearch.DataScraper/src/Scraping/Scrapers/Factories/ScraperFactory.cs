using JobSearch.DataScraper.Utils;

namespace JobSearch.DataScraper.Scraping.Scrapers.Factories;

public class ScraperFactory : IScraperFactory
{
	private readonly IServiceScopeFactory _serviceScopeFactory;
	private readonly ILogger<ScraperFactory> _logger;
	private readonly Dictionary<string, Type> _scraperTypes = new();
	
	public ScraperFactory(IServiceScopeFactory serviceScopeFactory, ILogger<ScraperFactory> logger)
	{
		_serviceScopeFactory = serviceScopeFactory;
		_logger = logger;
		RegisterScrapers();
	}
	
	private void RegisterScrapers()
	{
		foreach (var (name, binding) in ScraperHelper.TypeBindings)
			_scraperTypes[name] = binding.Type;
	}

	public IScraper CreateScraper(string scraperName)
	{
		if (!_scraperTypes.TryGetValue(scraperName, out var scraperType))
			throw new ArgumentException($"Scraper '{scraperName}' not found");
		
		using var scope = _serviceScopeFactory.CreateScope();
		return (IScraper)scope.ServiceProvider.GetRequiredService(scraperType);
	}
}