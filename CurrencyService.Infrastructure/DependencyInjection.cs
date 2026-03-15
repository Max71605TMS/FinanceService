using CurrencyService.Application.Interfaces;
using CurrencyService.Application.Services;
using CurrencyService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyService.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<CurrencyDbContext>(options =>
				options.UseNpgsql(
					configuration.GetConnectionString("Default"),
					b => b.MigrationsAssembly(typeof(CurrencyDbContext).Assembly.FullName)));


			services.AddScoped<ICurrencyRepository, CurrencyRepository>();
			services.AddScoped<ICurrencyService, CurrencyProviderService>();

			return services;
		}
	}
}
