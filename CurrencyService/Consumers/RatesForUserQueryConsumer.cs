using CurrencyService.Application.Interfaces;
using MassTransit;
using Shared.Dto;
using Shared.Queryes;

namespace CurrencyService.Consumers
{
	public class RatesForUserQueryConsumer : IConsumer<RatesForUserQuery>
	{
		private readonly ICurrencyService _currencyService;
		private readonly ILogger<RatesForUserQueryConsumer> _logger;

		public RatesForUserQueryConsumer(ICurrencyService currencyService, ILogger<RatesForUserQueryConsumer> logger)
		{
			_currencyService = currencyService;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<RatesForUserQuery> context)
		{
			_logger.LogInformation("Processing RatesForUserQuery for user {UserId}", context.Message.UserId);
			
			var rates = await _currencyService.GetUsersCurrenciesAsync(context.Message.UserId);

			if (rates == null) 
			{
				_logger.LogWarning("No rates found for user {UserId}", context.Message.UserId);
				return;
			}

			var response = new CurrencyRatesResponse
			{
				Rates = rates.Select(r => new CurrencyRateDto
				{
					CurrencyName = r.Name,
					Rate = r.Rate
				}).ToList()
			};

			_logger.LogInformation("Returning {Count} rates for user {UserId}",	response.Rates.Count, context.Message.UserId);
			
			await context.RespondAsync(response);
		}
	}
}
