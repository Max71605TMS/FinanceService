using CurrencyUpdater.Configuration;
using CurrencyUpdater.Infrastructure;
using CurrencyUpdater.Interfaces;
using CurrencyUpdater.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace CurrencyUpdater
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


			var builder = new HostApplicationBuilder();				

			builder.Services.Configure<CurrencySourceSettings>(
				builder.Configuration.GetSection("CurrencySourceSettings"));

			builder.Services.AddHttpClient<ICurrencyUpdatingService, CurrencyUpdatingService>();

			builder.Services.AddDbContext<CurrencyDbContext>(options =>
					options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

			
			builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
			builder.Services.AddHostedService<Worker>();

			builder.Services.AddLogging(configure =>
			{
				configure.AddConsole();
				configure.AddDebug();
			});

			var host = builder.Build();
			var configuration = new ConfigurationBuilder()
		.SetBasePath(AppContext.BaseDirectory)
		.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
		.AddEnvironmentVariables()
		.Build();

			// 2. Создаем объект настроек вручную
			var currencySettings = new CurrencySourceSettings();
			configuration.GetSection("CurrencySourceSettings").Bind(currencySettings);
			
			host.Run();
		}
	}
}