using JobSearch.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.Server
{
public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	public DbSet<UserEntity> Users => Set<UserEntity>();
	public DbSet<JobEntity> Jobs => Set<JobEntity>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}
}
}