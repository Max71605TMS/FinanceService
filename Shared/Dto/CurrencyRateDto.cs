namespace Shared.Dto
{
	public record CurrencyRateDto
	{
		public string CurrencyName { get; init; }
		public decimal Rate { get; init; }
	}
}
