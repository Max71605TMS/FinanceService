using CurrencyService.Application.Interfaces;
using CurrencyService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CurrencyService.Infrastructure.Repositories
{
	public class CurrencyRepository : ICurrencyRepository
	{
		private readonly CurrencyDbContext _context;
		private readonly ILogger<CurrencyRepository> _logger;

		public CurrencyRepository(CurrencyDbContext context, ILogger<CurrencyRepository> logger)
		{
			_context = context;
			_logger = logger;
		}
		public async Task<IEnumerable<Currency>?> GetByUserIdAsync(int id, CancellationToken cancellationToken = default)
		{
			_logger.LogInformation("Getting favorite currencies for user {UserId}", id);
			try
			{
				var query = from favorite in _context.Favorites
							join currency in _context.Сurrencies
								on favorite.CurrencyId equals currency.Id
							where favorite.UserId == id
							select currency;

				return await query.AsNoTracking().ToListAsync(cancellationToken);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting favorite currencies for user {UserId}", id);
				throw;
			}
		}
	}
}
