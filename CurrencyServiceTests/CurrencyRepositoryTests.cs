using CurrencyService.Domain;
using CurrencyService.Infrastructure;
using CurrencyService.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CurrencyServiceTests
{
	public class CurrencyRepositoryTests : IDisposable
	{
		private readonly CurrencyDbContext _context;
		private readonly CurrencyRepository _repository;
		private readonly Mock<ILogger<CurrencyRepository>> _loggerMock;

		public CurrencyRepositoryTests()
		{
			var options = new DbContextOptionsBuilder<CurrencyDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			_context = new CurrencyDbContext(options);
			_loggerMock = new Mock<ILogger<CurrencyRepository>>();
			_repository = new CurrencyRepository(_context, _loggerMock.Object);
		}

		public void Dispose()
		{
			_context.Dispose();
		}

		[Fact]
		public async Task GetByUserIdAsync_WithExistingFavorites_ShouldReturnCurrencies()
		{
			// Arrange
			var userId = 1;

			// Добавляем валюты
			var usd = new Currency { Id = 1, Name = "USD", Rate = 92.50m };
			var eur = new Currency { Id = 2, Name = "EUR", Rate = 100.20m };
			await _context.Сurrencies.AddRangeAsync(usd, eur);

			// Добавляем избранное
			await _context.Favorites.AddRangeAsync(
				new Favorite { UserId = userId, CurrencyId = 1 },
				new Favorite { UserId = userId, CurrencyId = 2 }
			);
			await _context.SaveChangesAsync();

			// Act
			var result = await _repository.GetByUserIdAsync(userId);

			// Assert
			result.Should().NotBeNull();
			result.Count().Should().Be(2);
			result.Should().Contain(c => c.Name == "USD" && c.Rate == 92.50m);
			result.Should().Contain(c => c.Name == "EUR" && c.Rate == 100.20m);
		}

		[Fact]
		public async Task GetByUserIdAsync_WithNoFavorites_ShouldReturnEmpty()
		{
			// Arrange
			var userId = 2;

			// Act
			var result = await _repository.GetByUserIdAsync(userId);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task GetByUserIdAsync_WithMixedFavorites_ShouldReturnOnlyUserCurrencies()
		{
			// Arrange
			var userId1 = 1;
			var userId2 = 2;

			var usd = new Currency { Id = 1, Name = "USD", Rate = 92.50m };
			var eur = new Currency { Id = 2, Name = "EUR", Rate = 100.20m };
			var gbp = new Currency { Id = 3, Name = "GBP", Rate = 117.80m };
			await _context.Сurrencies.AddRangeAsync(usd, eur, gbp);

			await _context.Favorites.AddRangeAsync(
				new Favorite { UserId = userId1, CurrencyId = 1 },
				new Favorite { UserId = userId1, CurrencyId = 2 },
				new Favorite { UserId = userId2, CurrencyId = 3 }
			);
			await _context.SaveChangesAsync();

			// Act
			var result = await _repository.GetByUserIdAsync(userId1);

			// Assert
			result.Should().NotBeNull();
			result.Count().Should().Be(2);
			result.Should().Contain(c => c.Name == "USD");
			result.Should().Contain(c => c.Name == "EUR");
			result.Should().NotContain(c => c.Name == "GBP");
		}

		[Fact]
		public async Task GetByUserIdAsync_WithInvalidUser_ShouldReturnEmpty()
		{
			// Arrange
			var userId = 999;

			// Act
			var result = await _repository.GetByUserIdAsync(userId);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeEmpty();
		}
	}
}
