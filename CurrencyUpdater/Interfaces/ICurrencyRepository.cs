namespace CurrencyUpdater.Interfaces
{
	internal interface ICurrencyRepository
	{
		Task UpdateCurrencyAsync(string name, decimal rate, CancellationToken cancellationToken = default);

		Task UpdateRangeAsync(IEnumerable<(string Name, decimal Rate)> currencies, CancellationToken cancellationToken = default);
		Task SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}
