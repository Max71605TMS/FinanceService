using CurrencyUpdater.Configuration;
using CurrencyUpdater.Interfaces;
using CurrencyUpdater.Models;
using Microsoft.Extensions.Options;
using System.Xml;
using System.Xml.Serialization;

namespace CurrencyUpdater.Services
{
	internal class CurrencyUpdatingService : ICurrencyUpdatingService
	{
		private readonly HttpClient _httpClient;
		private readonly ILogger<CurrencyUpdatingService> _logger;
		private readonly CurrencySourceSettings _currencySourceSettings;

		public CurrencyUpdatingService(HttpClient httpClient, 
									   ILogger<CurrencyUpdatingService> logger, 
									   IOptions<CurrencySourceSettings> currencySourceSettings)
		{
			_httpClient = httpClient;
			_logger = logger;
			_currencySourceSettings = currencySourceSettings.Value;
		}
		public async Task<List<(string Name, decimal Rate)>> GetCurrenciesAsync(CancellationToken cancellationToken = default)
		{
			_logger.LogInformation("Fetching currencies from CBR at {Url}", _currencySourceSettings.Url);
			try
			{
				var response = await _httpClient.GetAsync(_currencySourceSettings.Url, cancellationToken);
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync(cancellationToken);

				var serializer = new XmlSerializer(typeof(ValCurs));
				using var reader = new StringReader(content);
				var valCurs = (ValCurs?)serializer.Deserialize(reader);

				if (valCurs?.Valutes == null || !valCurs.Valutes.Any())
				{
					_logger.LogWarning("No currencies found in CBR response");
					return new List<(string, decimal)>();
				}

				var currencies = valCurs.Valutes
					.Where(v => !string.IsNullOrEmpty(v.Name))
					.Select(v => (v.CharCode, v.RateValue))
					.ToList();

				_logger.LogInformation("Successfully fetched {Count} currencies from CBR", currencies.Count);

				return currencies;
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "HTTP error fetching currencies from CBR");
				throw;
			}
			catch (XmlException ex)
			{
				_logger.LogError(ex, "XML parsing error processing CBR response");
				throw;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error fetching currencies from CBR");
				throw;
			}
		}
	}
}
