using JobSearch.Data;
using JobSearch.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.DataScraper.Data.Repositories;

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
		
		_logger.LogInformation($"[JobRepository] Create db context, id: {_db.Id}");
	}

	// start from key combination without hash (yet)

	public async Task<bool> ExistsAsync(JobEntity entity)
	{
		return await _db.Jobs
			.AsNoTracking()
			.AnyAsync(j => j.WebsiteSpecificId.Equals(entity.WebsiteSpecificId) && j.Url.Equals(entity.Url));
	}

	public async Task AddUniqueAsync(IEnumerable<JobEntity> models)
	{
		var jobModels = models as JobEntity[] ?? models.ToArray();
		
		if (!jobModels.Any())
			return;
		
		var incomingHashes = jobModels.Select(m => m.Url).ToHashSet();
		var existingHashes = await _db.Jobs
			.AsNoTracking()
			.Where(j => incomingHashes.Contains(j.Url))
			.Select(j => j.Url)
			.ToHashSetAsync();
		
		var uniqueModels = jobModels
			.Where(m => !existingHashes.Contains(m.Url))
			.ToList();
		
		if (!uniqueModels.Any())
			return;
    
		await _db.Jobs.AddRangeAsync(uniqueModels);

		_logger.LogInformation($"Added {uniqueModels.Count} new items to DB");
		
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

	public async Task RemoveNonExistentAsync(IEnumerable<JobEntity> models)
	{
		var jobModels = models as JobEntity[] ?? models.ToArray();
		
		if (!jobModels.Any())
			return;
		
		var website = jobModels.First().Website;
		var incomingHashes = jobModels.Select(m => m.Url).ToHashSet();
		
		var jobsToDelete = await _db.Jobs
			.Where(j => j.Website.Equals(website) && !incomingHashes.Contains(j.Url))
			.ToListAsync();

		if (!jobsToDelete.Any())
			return;
		
		_db.Jobs.RemoveRange(jobsToDelete);
		
		try
		{
			await _db.SaveChangesAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "deleting data from DB failed");
			throw;
		}
		
		_logger.LogInformation($"Removed {jobsToDelete.Count} items from DB");
	}
}