using CurrencyService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserService.Domain;

namespace MigrationService
{
	public class MigrationDBContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Currency> Currencies { get; set; }
		public DbSet<Favorite> Favorites { get; set; }

		public MigrationDBContext() { }		
		public MigrationDBContext(DbContextOptions<MigrationDBContext> options)
			: base(options) { }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{				
				var configuration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.Build();

				var connectionString = configuration.GetConnectionString("Default");
								
				optionsBuilder.UseNpgsql(connectionString);
			}
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>(entity =>
			{
				entity.ToTable("USERS");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
				entity.Property(e => e.Name).HasColumnName("NAME").IsRequired().HasMaxLength(100);
				entity.Property(e => e.Password).HasColumnName("PASSWORD").IsRequired().HasMaxLength(200);
				entity.HasIndex(e => e.Name).IsUnique();
			});

			modelBuilder.Entity<Currency>(entity =>
			{
				entity.ToTable("CURRENCIES");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
				entity.Property(e => e.Name).HasColumnName("NAME").IsRequired().HasMaxLength(100);
				entity.Property(e => e.Rate).HasColumnName("RATE").IsRequired().HasPrecision(18, 6);
				entity.HasIndex(e => e.Name).IsUnique();
			});

			modelBuilder.Entity<Favorite>(entity =>
			{
				entity.ToTable("FAVORITES");
				entity.HasKey(e => new { e.UserId, e.CurrencyId });
				entity.Property(e => e.UserId)
					.HasColumnName("USER_ID")
					.IsRequired();
				entity.Property(e => e.CurrencyId)
					.HasColumnName("CURRENCY_ID")
					.IsRequired();

				entity.HasIndex(e => e.UserId)
					.HasDatabaseName("IX_FAVORITES_USER_ID");

				entity.HasIndex(e => e.CurrencyId)
					.HasDatabaseName("IX_FAVORITES_CURRENCY_ID");

				entity.HasOne<User>()
					.WithMany()
					.HasForeignKey(e => e.UserId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne<Currency>()
					.WithMany()
					.HasForeignKey(e => e.CurrencyId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}
