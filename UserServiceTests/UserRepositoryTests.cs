using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UserService.Domain;
using UserService.Infrastructure;
using UserService.Infrastructure.Repositories;

namespace UserServiceTests
{
	public class UserRepositoryTests : IDisposable
	{
		private readonly UserDbContext _context;
		private readonly UserRepository _repository;

		public UserRepositoryTests()
		{
			var options = new DbContextOptionsBuilder<UserDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			_context = new UserDbContext(options);
			_repository = new UserRepository(_context);
		}

		[Fact]
		public async Task AddAsync_WithValidUser_ShouldReturnTrue()
		{
			// Arrange
			var user = new User("testuser", "password123");

			// Act
			var result = await _repository.AddAsync(user);

			// Assert
			result.Should().BeTrue();
			user.Id.Should().BeGreaterThan(0);
		}

		[Fact]
		public async Task AddAsync_ShouldPersistUserToDatabase()
		{
			// Arrange
			var user = new User("jane_doe", "1qw21qw2");

			// Act
			await _repository.AddAsync(user);

			// Assert
			var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Name == "jane_doe");
			savedUser.Should().NotBeNull();
			savedUser.Name.Should().Be("jane_doe");
			savedUser.Password.Should().Be("1qw21qw2");
		}

		[Fact]
		public async Task GetByNameAsync_WithExistingUser_ShouldReturnUser()
		{
			// Arrange
			var user = new User("find_me", "pass123");
			await _repository.AddAsync(user);

			// Act
			var found = await _repository.GetByNameAsync("find_me");

			// Assert
			found.Should().NotBeNull();
			found.Name.Should().Be("find_me");
			found.Password.Should().Be("pass123");
		}

		[Fact]
		public async Task GetByNameAsync_WithNonExistingUser_ShouldReturnNull()
		{
			// Act
			var found = await _repository.GetByNameAsync("nonexistent");

			// Assert
			found.Should().BeNull();
		}

		[Fact]
		public async Task GetByIdAsync_WithExistingUser_ShouldReturnUser()
		{
			// Arrange
			var user = new User("find_by_id", "pass123");
			await _repository.AddAsync(user);

			// Act
			var found = await _repository.GetByIdAsync(user.Id);

			// Assert
			found.Should().NotBeNull();
			found.Id.Should().Be(user.Id);
			found.Name.Should().Be("find_by_id");
		}

		[Fact]
		public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
		{
			// Act
			var found = await _repository.GetByIdAsync(999);

			// Assert
			found.Should().BeNull();
		}

		[Fact]
		public async Task MultipleAdds_ShouldWorkCorrectly()
		{
			// Arrange
			var user1 = new User("user1", "pass1");
			var user2 = new User("user2", "pass2");

			// Act
			var result1 = await _repository.AddAsync(user1);
			var result2 = await _repository.AddAsync(user2);

			// Assert
			result1.Should().BeTrue();
			result2.Should().BeTrue();

			var allUsers = await _context.Users.ToListAsync();
			allUsers.Count.Should().Be(2);
		}

		[Fact]
		public async Task GetByNameAsync_WithMultipleUsers_ShouldReturnCorrectOne()
		{
			// Arrange
			await _repository.AddAsync(new User("user1", "pass1"));
			await _repository.AddAsync(new User("user2", "pass2"));
			await _repository.AddAsync(new User("user3", "pass3"));

			// Act
			var found = await _repository.GetByNameAsync("user2");

			// Assert
			found.Should().NotBeNull();
			found.Name.Should().Be("user2");
			found.Password.Should().Be("pass2");
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
