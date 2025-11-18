using JobSearch.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.Server
{
public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	public DbSet<UserModel> Users => Set<UserModel>();
	public DbSet<JobModel> Jobs => Set<JobModel>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}
}
}