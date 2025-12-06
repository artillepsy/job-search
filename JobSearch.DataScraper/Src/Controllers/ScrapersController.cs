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
	private readonly IScraperBackgroundService _scraperBackgroundSerive;
	private readonly ScraperServiceConfigModel _configModel;

	public ScrapersController(
		IScraperBackgroundService scraperBackgroundService, 
		IOptions<ScraperServiceConfigModel> options,
		ILogger<ScrapersController> logger)
	{
		_logger = logger;
		_scraperBackgroundSerive = scraperBackgroundService;
		_configModel = options.Value;
	}

	[HttpGet("run-all")]
	public async Task<ActionResult> TestRun()
	{
		foreach (var scraper in _configModel.AllowedScrapers)
		{
			if (!scraper.IsEnabled)
			{
				continue;
			}

			try
			{
				await _scraperBackgroundSerive.StartScrapingAsync(scraper.Name);
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