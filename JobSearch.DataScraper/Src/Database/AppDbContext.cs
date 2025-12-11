using JobSearch.DataScraper.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.DataScraper.Database;

public class AppDbContext : DbContext
{
	public Guid Id { get; private set; }
	
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
		Id = Guid.NewGuid();
		Console.WriteLine($"[AppDbContext] Create DB context, id: {Id}");
	}
	
	public DbSet<JobModel> Jobs => Set<JobModel>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}

	public override void Dispose()
	{
		Console.WriteLine($"[AppDbContext] Dispose DB context, id: {Id}");
	}
}