using CurrencyService.Consumers;
using CurrencyService.Infrastructure;
using MassTransit;

namespace CurrencyService
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = Host.CreateApplicationBuilder(args);

			builder.Services.AddInfrastructure(builder.Configuration);

			builder.Services.AddMassTransit(x =>
			{
				x.AddConsumer<RatesForUserQueryConsumer>();

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

			var host = builder.Build();
			host.Run();
		}
	}
}