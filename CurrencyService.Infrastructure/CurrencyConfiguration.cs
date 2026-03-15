using CurrencyService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CurrencyService.Infrastructure
{
	public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
	{
		public void Configure(EntityTypeBuilder<Currency> builder)
		{
			builder.ToTable("CURRENCIES");
			builder.HasKey(c => c.Id);
			builder.Property(c => c.Id)
				.HasColumnName("ID")
				.ValueGeneratedOnAdd()
				.IsRequired();

			builder.Property(c => c.Name)
				.HasColumnName("NAME")
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(c => c.Rate)
				.HasColumnName("RATE")
				.IsRequired()
				.HasPrecision(18, 6); 

			builder.HasIndex(c => c.Name)
				.IsUnique()
				.HasDatabaseName("ix_currencies_name");
		}
	}
}
