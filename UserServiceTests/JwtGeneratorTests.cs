using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using UserService.Domain;
using UserService.Infrastructure;

namespace UserServiceTests
{
	public class JwtGeneratorTests
	{
		private readonly Mock<IConfiguration> _configMock;
		private readonly JwtGenerator _jwtGenerator;

		public JwtGeneratorTests()
		{
			_configMock = new Mock<IConfiguration>();
			
			_configMock.Setup(x => x["Jwt:Key"]).Returns("super-secret-key-that-is-long-enough-32-chars!");
			_configMock.Setup(x => x["Jwt:Issuer"]).Returns("test-issuer");
			_configMock.Setup(x => x["Jwt:Audience"]).Returns("test-audience");

			_jwtGenerator = new JwtGenerator(_configMock.Object);
		}

		[Fact]
		public void GenerateToken_WithValidUser_ShouldReturnToken()
		{
			// Arrange
			var user = new User("john_doe", "password123");
			var userWithId = new User("john_doe", "password123") { Id = 1 };

			// Act
			var token = _jwtGenerator.GenerateToken(userWithId);

			// Assert
			token.Should().NotBeNullOrEmpty();
			
			var handler = new JwtSecurityTokenHandler();
			var jwtToken = handler.ReadJwtToken(token);

			jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "1");
			jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == "john_doe");
			jwtToken.Issuer.Should().Be("test-issuer");
			jwtToken.Audiences.Should().Contain("test-audience");
		}
	}
}
