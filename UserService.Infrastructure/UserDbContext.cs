using Microsoft.EntityFrameworkCore;
using UserService.Domain;

namespace UserService.Infrastructure
{
	public class UserDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }

		public UserDbContext(DbContextOptions<UserDbContext> options)
			: base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
			base.OnModelCreating(modelBuilder);
		}

	}
}
