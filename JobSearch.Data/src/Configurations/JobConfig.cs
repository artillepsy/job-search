using JobSearch.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobSearch.Data.Configurations;

public class JobConfig : IEntityTypeConfiguration<JobEntity>
{
	public void Configure(EntityTypeBuilder<JobEntity> builder)
	{
		builder.ToTable("jobs");
		builder.HasKey(e => e.Id);
		builder.Property(e => e.Id).UseIdentityByDefaultColumn();
		builder.Property(e => e.Title).IsRequired();
		builder.Property(e => e.CompanyName).IsRequired();
		builder.Property(e => e.Website).IsRequired();
		builder.Property(e => e.Url).IsRequired();
		builder.Property(e => e.CreatedAt).IsRequired();
		builder.HasIndex(e => e.CreatedAt);
	}
}