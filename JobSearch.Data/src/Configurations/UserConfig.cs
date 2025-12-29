using JobSearch.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobSearch.Data.Configurations;

public class UserConfig : IEntityTypeConfiguration<UserEntity>
{
	public void Configure(EntityTypeBuilder<UserEntity> builder)
	{
		builder.ToTable("users");
		builder.HasKey(e => e.Id);
		builder.Property(e => e.Id).UseIdentityByDefaultColumn();
		builder.Property(e => e.Username).IsRequired();
		builder.Property(e => e.PasswordHash).IsRequired();
	}
}