using JobSearch.DataScraper.Scraping;
using Microsoft.AspNetCore.Mvc;

namespace JobSearch.DataScraper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScrapersController : ControllerBase
{
	private readonly ILogger<ScrapersController> _logger;
	private readonly ScraperBackgroundService _scraperService;

	public ScrapersController(
		ScraperBackgroundService scraperService, 
		ILogger<ScrapersController> logger)
	{
		_logger = logger;
		_scraperService = scraperService;
	}

	[HttpGet("run-all")]
	public async Task<ActionResult> RunAll()
	{
		await _scraperService.RunAllEnabledScrapersAsync();
		return Accepted();
	}

	[HttpGet("run/{name}")]
	public async Task<ActionResult> RunSingle(string name)
	{
		// You can still run a single one if needed
		await _scraperService.StartScrapingAsync(name);
		return Accepted();
	}
}