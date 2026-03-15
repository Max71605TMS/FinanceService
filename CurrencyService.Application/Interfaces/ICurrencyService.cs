using CurrencyService.Domain;

namespace CurrencyService.Application.Interfaces
{
	public interface ICurrencyService
	{
		Task<IReadOnlyCollection<Currency>?> GetUsersCurrenciesAsync(int id);
	}
}
