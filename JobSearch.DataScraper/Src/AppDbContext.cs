using JobSearch.DataScraper.Models;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.DataScraper;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
	
	public DbSet<JobModel> Jobs => Set<JobModel>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}
}