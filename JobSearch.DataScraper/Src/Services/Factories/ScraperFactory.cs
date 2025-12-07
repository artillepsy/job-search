using JobSearch.DataScraper.Services.Scrapers;
using JobSearch.DataScraper.Services.Utils;

namespace JobSearch.DataScraper.Services.Factories;

public class ScraperFactory : IScraperFactory
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<ScraperFactory> _logger;
	private readonly Dictionary<string, Type> _scraperTypes = new();
	
	public ScraperFactory(IServiceProvider serviceProvider, ILogger<ScraperFactory> logger)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
		RegisterScrapers();
	}
	
	private void RegisterScrapers()
	{
		foreach (var (name, binding) in ScraperUtils.Bindings)
			_scraperTypes[name] = binding.Type;
	}

	public IScraper CreateScraper(string scraperName)
	{
		if (!_scraperTypes.TryGetValue(scraperName, out var scraperType))
			throw new ArgumentException($"Scraper '{scraperName}' not found");
		
		using var scope = _serviceProvider.CreateScope();
		return (IScraper)scope.ServiceProvider.GetRequiredService(scraperType);
	}
}