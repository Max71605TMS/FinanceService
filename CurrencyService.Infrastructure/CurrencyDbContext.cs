using CurrencyService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CurrencyService.Infrastructure
{
	public class CurrencyDbContext : DbContext
	{
		public DbSet<Currency> Сurrencies { get; set; }

		public DbSet<Favorite> Favorites { get; set; }

		public CurrencyDbContext(DbContextOptions<CurrencyDbContext> options)
			: base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(CurrencyDbContext).Assembly);
			modelBuilder.Entity<Favorite>(entity =>
			{
				entity.HasKey(f => new { f.UserId, f.CurrencyId });
			});
			base.OnModelCreating(modelBuilder);
		}
	}
}
