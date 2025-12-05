using JobSearch.DataScraper.Services.Core.Factory;
using Microsoft.AspNetCore.Mvc;

namespace JobSearch.DataScraper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScrapersController : ControllerBase
{
	private readonly AppDbContext _db;

	private IScraperFactory _factory;

	public ScrapersController()
	{
		
	}

}