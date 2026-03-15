using CurrencyService.Application.Interfaces;
using CurrencyService.Application.Services;
using CurrencyService.Domain;
using FluentAssertions;
using Moq;

namespace CurrencyServiceTests
{
	public class CurrencyProviderServiceTests
	{
		private readonly Mock<ICurrencyRepository> _repositoryMock;
		private readonly CurrencyProviderService _service;

		public CurrencyProviderServiceTests()
		{
			_repositoryMock = new Mock<ICurrencyRepository>();			
			_service = new CurrencyProviderService(_repositoryMock.Object);
		}

		[Fact]
		public async Task GetUsersCurrencies_WithValidUserId_ShouldReturnCurrencies()
		{
			// Arrange
			var userId = 1;
			var expectedCurrencies = new List<Currency>
		{
			new() { Id = 1, Name = "USD", Rate = 92.50m },
			new() { Id = 2, Name = "EUR", Rate = 100.20m }
		};

			_repositoryMock.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(expectedCurrencies);

			// Act
			var result = await _service.GetUsersCurrenciesAsync(userId);

			// Assert
			result.Should().NotBeNull();
			result.Count.Should().Be(2);
			var resultList = result.ToList();
			resultList[0].Name.Should().Be("USD");
			resultList[0].Rate.Should().Be(92.50m);
			resultList[1].Name.Should().Be("EUR");
			resultList[1].Rate.Should().Be(100.20m);

			_repositoryMock.Verify(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task GetUsersCurrencies_WithNoFavorites_ShouldReturnEmptyList()
		{
			// Arrange
			var userId = 2;
			_repositoryMock.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<Currency>());

			// Act
			var result = await _service.GetUsersCurrenciesAsync(userId);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetUsersCurrencies_WhenRepositoryReturnsNull_ShouldReturnEmptyList()
		{
			// Arrange
			var userId = 3;
			_repositoryMock.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
				.ReturnsAsync((IEnumerable<Currency>?)null);

			// Act
			var result = await _service.GetUsersCurrenciesAsync(userId);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetUsersCurrencies_WhenRepositoryThrows_ShouldRethrow()
		{
			// Arrange
			var userId = 4;
			_repositoryMock.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
				.ThrowsAsync(new Exception("Database error"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(() => _service.GetUsersCurrenciesAsync(userId));
		}
	}
}