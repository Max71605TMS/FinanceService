using CurrencyService.Application.Interfaces;
using CurrencyService.Domain;

namespace CurrencyService.Application.Services
{
	public class CurrencyProviderService : ICurrencyService
	{
		private readonly ICurrencyRepository _repository;
		public CurrencyProviderService(ICurrencyRepository repository)
		{
			_repository = repository;
		}
		public async Task<IReadOnlyCollection<Currency>?> GetUsersCurrenciesAsync(int id)
		{
			var currencies = await _repository.GetByUserIdAsync(id);
			return currencies?.ToList().AsReadOnly() ?? new List<Currency>().AsReadOnly();
		}
	}
}
