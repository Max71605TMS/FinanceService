using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain;

namespace UserService.Infrastructure
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.ToTable("USERS");

			builder.HasKey(u => u.Id);
			builder.Property(u => u.Id)
				.HasColumnName("ID")
				.ValueGeneratedOnAdd();

			builder.Property(u => u.Name)
				.HasColumnName("NAME")
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(u => u.Password)
				.HasColumnName("PASSWORD")
				.IsRequired()
				.HasMaxLength(200);

			builder.HasIndex(u => u.Name)
				.IsUnique()
				.HasDatabaseName("ix_users_name");
		}
	}
}
