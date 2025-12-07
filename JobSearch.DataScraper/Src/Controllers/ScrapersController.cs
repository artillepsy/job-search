using JobSearch.DataScraper.Services.Background;
using JobSearch.DataScraper.Services.ConfigurationModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JobSearch.DataScraper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScrapersController : ControllerBase
{
	//private readonly AppDbContext _db;

	private readonly ILogger<ScrapersController> _logger;
	private readonly IScraperBackgroundService _scraperBackgroundService;
	private readonly IOptions<ScraperServiceConfigModel> _options;

	public ScrapersController(
		IScraperBackgroundService scraperBackgroundService, 
		IOptions<ScraperServiceConfigModel> options,
		ILogger<ScrapersController> logger)
	{
		_logger = logger;
		_scraperBackgroundService = scraperBackgroundService;
		_options = options;
	}

	[HttpGet("run-all")]
	public async Task<ActionResult> TestRun()
	{
		_logger.LogInformation($"config: {_options.Value}");
		
		foreach (var scraper in _options.Value.AllowedScrapers)
		{
			if (!scraper.IsEnabled)
			{
				continue;
			}

			try
			{
				await _scraperBackgroundService.StartScrapingAsync(scraper.Name);
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"scraper failed to run: {scraper.Name}");
				throw;
			}
		}

		return Ok();
	}

}