using CurrencyService.Application.Interfaces;
using CurrencyService.Consumers;
using CurrencyService.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Queryes;

namespace CurrencyServiceTests
{
	public class RatesForUserQueryConsumerTests
	{
		private readonly Mock<ICurrencyService> _currencyServiceMock;
		private readonly Mock<ILogger<RatesForUserQueryConsumer>> _loggerMock;
		private readonly RatesForUserQueryConsumer _consumer;
		private readonly Mock<ConsumeContext<RatesForUserQuery>> _contextMock;

		public RatesForUserQueryConsumerTests()
		{
			_currencyServiceMock = new Mock<ICurrencyService>();
			_loggerMock = new Mock<ILogger<RatesForUserQueryConsumer>>();
			_consumer = new RatesForUserQueryConsumer(_currencyServiceMock.Object, _loggerMock.Object);
			_contextMock = new Mock<ConsumeContext<RatesForUserQuery>>();
		}

		[Fact]
		public async Task Consume_WithValidUserId_ShouldReturnRates()
		{
			// Arrange
			var userId = 1;
			var query = new RatesForUserQuery { UserId = userId };
			IReadOnlyCollection<Currency> expectedRates = new List<Currency>
					{
						new() { Id = 1, Name = "USD", Rate = 92.50m },
						new() { Id = 2, Name = "EUR", Rate = 100.20m }
					};

			_contextMock.Setup(x => x.Message).Returns(query);

			_currencyServiceMock.Setup(x => x.GetUsersCurrenciesAsync(It.IsAny<int>()))
								.ReturnsAsync(expectedRates);

			// Act
			await _consumer.Consume(_contextMock.Object);

			// Assert
			_contextMock.Verify(x => x.RespondAsync(It.Is<CurrencyRatesResponse>(r =>
				r.Rates.Count == 2 &&
				r.Rates[0].CurrencyName == "USD" &&
				r.Rates[0].Rate == 92.50m &&
				r.Rates[1].CurrencyName == "EUR")),
				Times.Once);
		}
		
		[Fact]
		public async Task Consume_WhenServiceThrows_ShouldThrow()
		{
			// Arrange
			var userId = 3;
			var query = new RatesForUserQuery { UserId = userId };

			_contextMock.Setup(x => x.Message).Returns(query);
			_currencyServiceMock.Setup(x => x.GetUsersCurrenciesAsync(userId))
				.ThrowsAsync(new Exception("Service error"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(() => _consumer.Consume(_contextMock.Object));
		}
	}
}
