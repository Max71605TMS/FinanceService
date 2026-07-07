using FluentAssertions;
using UserService.Application.Services;

namespace UserServiceTests
{
	public class Pbkdf2PasswordHasherTests
	{
		[Fact]
		public void HashPassword_ShouldCreateVerifiableHashWithoutStoringPlainPassword()
		{
			// Arrange
			var hasher = new Pbkdf2PasswordHasher();
			var password = "password123";

			// Act
			var passwordHash = hasher.HashPassword(password);

			// Assert
			passwordHash.Should().NotBe(password);
			passwordHash.Should().StartWith("PBKDF2-SHA256$100000$");
			hasher.VerifyPassword(password, passwordHash).Should().BeTrue();
			hasher.VerifyPassword("wrong-password", passwordHash).Should().BeFalse();
		}
	}
}
