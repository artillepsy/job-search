using JobSearch.DataScraper.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.DataScraper.Database.Repositories;

//todo: probably there should be a unique composite key. Because if we need a quick access to a database,
// it should take a very little time? hashed url + job website specific id? it will occupy more space tho - think about it
public class JobRepository : IJobRepository
{
	private readonly AppDbContext _db;
	private readonly ILogger<JobRepository> _logger;

	private class Sha1UrlSelection
	{
		public int Id { get; set; }
		public string Sha1UrlHash { get; set; } = "";
	}

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

	public async Task AddUniqueAsync(IEnumerable<JobModel> models)
	{
		if (!models.Any())
			return;
		
		var existingHashes = await _db.Jobs
			.AsNoTracking()
			.Where(j => models.Select(m => m.Sha1UrlHash).Contains(j.Sha1UrlHash))
			.Select(j => j.Sha1UrlHash)
			.ToHashSetAsync();
		
		var uniqueModels = models
			.Where(m => !existingHashes.Contains(m.Sha1UrlHash))
			.ToList();
		
		if (!uniqueModels.Any())
			return;
    
		await _db.Jobs.AddRangeAsync(uniqueModels);

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

	public async Task RemoveNonExistentAsync(IEnumerable<JobModel> models)
	{
		if (!models.Any())
			return;
		
		var website = models.First().Website;
		var incomingHashes = models.Select(m => m.Sha1UrlHash).ToHashSet();
		var idsToDelete = await _db.Jobs
			.AsNoTracking()
			.Where(j => j.Website.Equals(website) && !incomingHashes.Contains(j.Sha1UrlHash))
			.Select(j => j.Id)
			.ToListAsync();

		if (!idsToDelete.Any())
			return;
		
		await _db.Jobs
			.Where(j => idsToDelete.Contains(j.Id))
			.ExecuteDeleteAsync();
	}
}