using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MigrationService
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = Host.CreateApplicationBuilder(args);			

			builder.Services.AddDbContext<MigrationDBContext>(options =>
				options.UseNpgsql(
					builder.Configuration.GetConnectionString("Default"),
					b => b.MigrationsAssembly(typeof(MigrationDBContext).Assembly.FullName)));

			var host = builder.Build();

			using (var scope = host.Services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<MigrationDBContext>();
				await dbContext.Database.MigrateAsync();
			}
		}
	}
}
