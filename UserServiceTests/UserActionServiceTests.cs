using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Application.Services;
using UserService.Domain;

namespace UserServiceTests
{
	public class UserActionServiceTests
	{
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<IJwtGenerator> _jwtGeneratorMock;
		private readonly Mock<IDistributedCache> _cacheMock;
		private readonly UserActionService _service;

		public UserActionServiceTests()
		{
			_userRepositoryMock = new Mock<IUserRepository>();
			_jwtGeneratorMock = new Mock<IJwtGenerator>();
			_cacheMock = new Mock<IDistributedCache>();			

			_service = new UserActionService(
				_userRepositoryMock.Object,
				_cacheMock.Object,          
				_jwtGeneratorMock.Object);
		}

		[Fact]
		public async Task RegisterUser_WithValidData_ShouldReturnSuccess()
		{
			// Arrange
			var name = "newuser";
			var password = "password123"; 
			
			_userRepositoryMock.Setup(x => x.GetByNameAsync(name, It.IsAny<CancellationToken>()))
				.ReturnsAsync((User?)null);
			
			_userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(true); 

			// Act
			var result = await _service.RegisterUserAsync(name, password);

			// Assert
			result.Should().NotBeNull();
			
			_userRepositoryMock.Verify(x => x.AddAsync(
				It.Is<User>(u => u.Name == name && u.Password == password),
				It.IsAny<CancellationToken>()),
				Times.Once);

			_cacheMock.Verify(x => x.SetAsync(
			It.IsAny<string>(),
			It.IsAny<byte[]>(),
			It.IsAny<DistributedCacheEntryOptions>(),
			It.IsAny<CancellationToken>()),
			Times.Never);
		}

		[Fact]
		public async Task Login_WithValidCredentials_ShouldReturnToken()
		{
			// Arrange
			var name = "john";
			var password = "pass123";
			var user = new User(name, password);
			var token = "jwt_token";

			_userRepositoryMock.Setup(x => x.GetByNameAsync(name, It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);
			_jwtGeneratorMock.Setup(x => x.GenerateToken(user))
				.Returns(token);

			// Act
			var result = await _service.LoginAsync(name, password);

			// Assert
			result.IsSuccessful.Should().BeTrue();
			result.Token.Should().Be(token);
			result.Name.Should().Be(name);

			_cacheMock.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
				Times.Never);
			_cacheMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(),
				It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()),
				Times.Never);
		}

		[Fact]
		public async Task Login_WithInvalidPassword_ShouldThrowException()
		{
			// Arrange
			var name = "john";
			var password = "wrongpass";
			var user = new User(name, "correctpass");

			_userRepositoryMock.Setup(x => x.GetByNameAsync(name, It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);

			// Act & Assert
			await Assert.ThrowsAsync<UnauthorizedException>(
				() => _service.LoginAsync(name, password));
		}

		[Fact]
		public async Task Login_WithNonExistentUser_ShouldThrowException()
		{
			// Arrange
			var name = "unknown";
			var password = "pass123";

			_userRepositoryMock.Setup(x => x.GetByNameAsync(name, It.IsAny<CancellationToken>()))
				.ReturnsAsync((User?)null);

			// Act & Assert
			await Assert.ThrowsAsync<UnauthorizedException>(
				() => _service.LoginAsync(name, password));
		}

		[Fact]
		public async Task Logout_ShouldAddTokenToBlacklist()
		{
			// Arrange
			var userId = 1;
			var token = "jwt_token";
			var user = new User("john", "pass");
			
			_userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);
			
			// Act
			var result = await _service.LogoutAsync(userId, token);

			// Assert
			result.Success.Should().BeTrue();
			
			_cacheMock.Verify(x => x.SetAsync(
				It.Is<string>(key => key == $"blacklist:{token}"),
				It.IsAny<byte[]>(),
				It.IsAny<DistributedCacheEntryOptions>(),
				It.IsAny<CancellationToken>()),
				Times.Once);
		}

		[Fact]
		public void GetTokenExpiration_WithValidToken_ShouldReturnExpirationDate()
		{
			// Arrange
			var expectedExpiration = DateTime.UtcNow.AddHours(2);
			var token = GenerateTestToken(expectedExpiration);

			// Act
			var result = InvokeGetTokenExpiration(token);

			// Assert
			result.Should().BeCloseTo(expectedExpiration, precision: TimeSpan.FromSeconds(1));
		}
		private DateTime InvokeGetTokenExpiration(string token)
		{
			var method = typeof(UserActionService).GetMethod("GetTokenExpiration",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

			return (DateTime)method.Invoke(_service, new object[] { token });
		}

		private string GenerateTestToken(DateTime expiration)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes("test-key-32-chars-long-for-test-using-test");

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
				new Claim(JwtRegisteredClaimNames.Sub, "1"),
				new Claim(JwtRegisteredClaimNames.UniqueName, "testuser")
			}),
				Expires = expiration,
				Issuer = "test-issuer",
				Audience = "test-audience",
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(key),
					SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
