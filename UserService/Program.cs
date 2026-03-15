using MassTransit;
using Microsoft.EntityFrameworkCore;
using UserService.Consumers;
using UserService.Infrastructure;


namespace UserService
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = Host.CreateApplicationBuilder(args);
			builder.Services.AddInfrastructure(builder.Configuration);
			
			builder.Services.AddMassTransit(x =>
			{								
				x.AddConsumer<RegisterUserConsumer>();
				x.AddConsumer<LoginUserConsumer>();
				x.AddConsumer<LogoutUserConsumer>();

				x.UsingRabbitMq((context, cfg) =>
				{
					cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
					{
						h.Username(builder.Configuration["RabbitMQ:Username"]);
						h.Password(builder.Configuration["RabbitMQ:Password"]);
					});

					cfg.ConfigureEndpoints(context);
				});
			});

			builder.Services.AddStackExchangeRedisCache(options =>
			{
				options.Configuration = builder.Configuration.GetConnectionString("Redis");
				options.InstanceName = "TokenBlacklist";
			});

			var host = builder.Build();
			
			host.Run();
		}
	}
}