using CurrencyUpdater.Configuration;
using CurrencyUpdater.Interfaces;
using Microsoft.Extensions.Options;

namespace CurrencyUpdater
{
	/// <summary>
	/// Сервис который занимается обновлением курсов валют
	/// </summary>
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;
		private readonly IServiceProvider _serviceProvider;
		private readonly UpdateSettings _updateSettings;

		public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IOptions<UpdateSettings> updateSettings)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
			_updateSettings = updateSettings.Value;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Currency Updater Service started at: {time}", DateTime.Now);
			_logger.LogInformation("Update interval: {interval}", _updateSettings.Interval);

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					await UpdateCurrenciesAsync(stoppingToken);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred while updating currencies");
				}

				_logger.LogInformation("Next update scheduled at: {time}", DateTime.Now.Add(_updateSettings.Interval));
				await Task.Delay(_updateSettings.Interval, stoppingToken);
			}
		}

		private async Task UpdateCurrenciesAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Starting currency update at: {time}", DateTime.Now);

			using var scope = _serviceProvider.CreateScope();
			var cbrService = scope.ServiceProvider.GetRequiredService<ICurrencyUpdatingService>();
			var repository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();
										
			var currencies = await cbrService.GetCurrenciesAsync(cancellationToken);

			if (!currencies.Any())
			{
				_logger.LogWarning("No currencies to update");
				return;
			}

			await repository.UpdateRangeAsync(currencies, cancellationToken);
			await repository.SaveChangesAsync();
			_logger.LogInformation("Successfully updated {Count} currencies at: {time}",
			currencies.Count, DateTime.Now);
		}		
	}
}