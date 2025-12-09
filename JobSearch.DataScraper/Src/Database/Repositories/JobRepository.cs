using JobSearch.DataScraper.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.DataScraper.Database.Repositories;

//todo: probably there should be a unique composite key. Because if we need a quick access to a database,
// it should take a very little time? hashed url + job website specific id? it will occupy more space tho - think about it
public class JobRepository : IJobRepository
{
	private readonly AppDbContext _db;
	private readonly ILogger<JobRepository> _logger;

	public JobRepository(AppDbContext db, ILogger<JobRepository> logger)
	{
		_db = db;
		_logger = logger;
	}

	// start from key combination without hash (yet)
	public async Task<bool> ExistsAsync(JobModel model)
	{
		return await _db.Jobs
			.AsNoTracking()
			.AnyAsync(j => j.WebsiteSpecificId.Equals(model.WebsiteSpecificId) && j.Url.Equals(model.Url));
	}

	public async Task AddRangeAsync(IEnumerable<JobModel> models)
	{
		await _db.Jobs.AddRangeAsync(models);

		try
		{
			await _db.SaveChangesAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "writing data to DB failed");
			throw;
		}
	}
}