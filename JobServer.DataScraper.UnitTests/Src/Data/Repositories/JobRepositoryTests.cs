using JobSearch.DataScraper.Data;
using JobSearch.DataScraper.Data.Entities;
using JobSearch.DataScraper.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace JobServer.DataScraper.UnitTests.Data.Repositories;

public class JobRepositoryTests
{
	private AppDbContext CreateAppDbContext()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;

		return new AppDbContext(options);
	}

	private JobRepository CreateRepository(AppDbContext ctx)
	{
		var mockLogger = new Mock<ILogger<JobRepository>>();
		return new JobRepository(ctx, mockLogger.Object);
	}

	[Fact]
	public async Task AddUniqueAsync_Adds_Only_New_Items()
	{
		var ctx = CreateAppDbContext();
		var repository = CreateRepository(ctx);

		var existingJobs = new List<JobEntity>()
		{
			new () { Sha1UrlHash = "1" },
			new () { Sha1UrlHash = "2" },
		};
		
		var incomingJobs = new List<JobEntity>()
		{
			new () { Sha1UrlHash = "2" },
			new () { Sha1UrlHash = "3" },
		};
		
		ctx.Jobs.AddRange(existingJobs);
		await ctx.SaveChangesAsync();

		await repository.AddUniqueAsync(incomingJobs);

		var allJobs = await ctx.Jobs.ToListAsync();
		
		Assert.Equal(3, allJobs.Count);
		Assert.Contains(allJobs, j => j.Sha1UrlHash.Equals("3"));
		Assert.Single(allJobs, j => j.Sha1UrlHash.Equals("2"));
	}
}