using Shared.Dto;

namespace Shared.Queryes
{
	public record CurrencyRatesResponse
	{
		public List<CurrencyRateDto> Rates { get; init; }
	}
}
