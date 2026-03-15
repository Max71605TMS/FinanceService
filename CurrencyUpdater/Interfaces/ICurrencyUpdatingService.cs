namespace CurrencyUpdater.Interfaces
{
	internal interface ICurrencyUpdatingService
	{
		Task<List<(string Name, decimal Rate)>> GetCurrenciesAsync(CancellationToken cancellationToken = default);
	}
}
