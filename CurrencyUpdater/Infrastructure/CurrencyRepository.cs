using CurrencyService.Domain;
using CurrencyUpdater.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CurrencyUpdater.Infrastructure
{
	internal class CurrencyRepository : ICurrencyRepository
	{
		private readonly CurrencyDbContext _context;
		private readonly ILogger<CurrencyRepository> _logger;

		public CurrencyRepository(CurrencyDbContext context, ILogger<CurrencyRepository> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			await _context.SaveChangesAsync(cancellationToken);
		}

		public async Task UpdateCurrencyAsync(string name, decimal rate, CancellationToken cancellationToken = default)
		{
			var currency = await _context.Currencies
			.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);

			if (currency == null)
			{
				currency = new Currency
				{
					Name = name,
					Rate = rate,					
				};
				await _context.Currencies.AddAsync(currency, cancellationToken);
				_logger.LogInformation("Added new currency: {Name} with rate {Rate}", name, rate);
			}
			else
			{
				currency.Rate = rate;				
				_logger.LogInformation("Updated currency: {Name} new rate {Rate}", name, rate);
			}			
		}

		public async Task UpdateRangeAsync(IEnumerable<(string Name, decimal Rate)> currencies, CancellationToken cancellationToken = default)
		{
			var currencyList = currencies.ToList();
			if (!currencyList.Any())
			{
				_logger.LogWarning("No currencies to update");
				return;
			}

			_logger.LogInformation("Starting update for {Count} currencies", currencyList.Count);

			var existingCurrencies = await _context.Currencies
				.ToDictionaryAsync(c => c.Name, cancellationToken);

			foreach (var (name, rate) in currencyList)
			{
				if (existingCurrencies.TryGetValue(name, out var existingCurrency))
				{
					existingCurrency.Rate = rate;
				}
				else
				{
					await _context.Currencies.AddAsync(new Currency
					{
						Name = name,
						Rate = rate
					}, cancellationToken);
				}
			}

			_logger.LogInformation("Prepared {Count} currencies for update", currencyList.Count);
		}
	}
}
