using JobSearch.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.Data;

public class AppDbContext : DbContext
{
	public Guid Id { get; }
	
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
		Id = Guid.NewGuid();
		Console.WriteLine($"[AppDbContext] Create DB context, id: {Id}");
	}
	
	public DbSet<JobEntity> Jobs => Set<JobEntity>();
	public DbSet<UserEntity> Users => Set<UserEntity>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}

	public override void Dispose()
	{
		Console.WriteLine($"[AppDbContext] Dispose DB context, id: {Id}");
	}
}