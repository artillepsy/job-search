using JobSearch.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.DataScraper.Data;

public class AppDbContext : DbContext
{
	public Guid Id { get; }
	
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
		Id = Guid.NewGuid();
		Console.WriteLine($"[AppDbContext] Create DB context, id: {Id}");
	}
	
	public DbSet<JobEntity> Jobs => Set<JobEntity>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<JobEntity>(entity =>
		{
			entity.ToTable("jobs");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Id).UseIdentityByDefaultColumn();
			entity.Property(e => e.Title).IsRequired();
			entity.Property(e => e.CompanyName).IsRequired();
			entity.Property(e => e.Website).IsRequired();
			entity.Property(e => e.Url).IsRequired();
			entity.Property(e => e.CreatedAt).IsRequired();
			entity.HasIndex(e => e.CreatedAt);
		});
		
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}

	public override void Dispose()
	{
		Console.WriteLine($"[AppDbContext] Dispose DB context, id: {Id}");
	}
}