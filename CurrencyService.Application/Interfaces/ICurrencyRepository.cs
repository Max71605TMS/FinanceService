using CurrencyService.Domain;

namespace CurrencyService.Application.Interfaces
{
	public interface ICurrencyRepository
	{
		Task<IEnumerable<Currency>?> GetByUserIdAsync(int id, CancellationToken cancellationToken = default);
	}
}
