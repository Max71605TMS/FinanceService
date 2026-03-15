using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Repositories;
using UserService.Application.Services;

namespace UserService.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{			
			services.AddDbContext<UserDbContext>(options =>
				options.UseNpgsql(
					configuration.GetConnectionString("Default"),
					b => b.MigrationsAssembly(typeof(UserDbContext).Assembly.FullName)));

			
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IUserActionService, UserActionService>();
			services.AddScoped<IJwtGenerator, JwtGenerator>();

			return services;
		}
	}
}
