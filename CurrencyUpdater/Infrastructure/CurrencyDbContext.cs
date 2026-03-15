using CurrencyService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CurrencyUpdater.Infrastructure
{
	internal class CurrencyDbContext : DbContext
	{
		public DbSet<Currency> Currencies { get; set; }

		public CurrencyDbContext(DbContextOptions<CurrencyDbContext> options)
			: base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Currency>(entity =>
			{
				entity.ToTable("CURRENCIES");

				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id)
					.HasColumnName("ID")
					.ValueGeneratedOnAdd();

				entity.Property(e => e.Name)
					.HasColumnName("NAME")
					.IsRequired()
					.HasMaxLength(100);

				entity.Property(e => e.Rate)
					.HasColumnName("RATE")
					.IsRequired()
					.HasPrecision(18, 6);				

				entity.HasIndex(e => e.Name)
					.IsUnique()
					.HasDatabaseName("IX_CURRENCIES_NAME");
			});
		}
	}
}
