using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Commands;
using Shared.Queryes;
using UserService.Application.Interfaces;
using UserService.Consumers;

namespace UserServiceTests
{
	public class LoginUserConsumerTests
	{
		private readonly Mock<IUserActionService> _userServiceMock;
		private readonly Mock<ILogger<LoginUserConsumer>> _loggerMock;
		private readonly LoginUserConsumer _consumer;
		private readonly Mock<ConsumeContext<LoginUserCommand>> _contextMock;

		public LoginUserConsumerTests()
		{
			_userServiceMock = new Mock<IUserActionService>();
			_loggerMock = new Mock<ILogger<LoginUserConsumer>>();
			_consumer = new LoginUserConsumer(_userServiceMock.Object, _loggerMock.Object);
			_contextMock = new Mock<ConsumeContext<LoginUserCommand>>();
		}

		[Fact]
		public async Task Consume_WithValidCredentials_ShouldRespondWithSuccess()
		{
			// Arrange
			var command = new LoginUserCommand
			{
				Name = "john",
				Password = "pass123"
			};

			var expectedResponse = new LoginResponse
			{
				IsSuccessful = true,
				Name = "john",
				Token = "jwt_token"
			};

			_contextMock.Setup(x => x.Message).Returns(command);
			_userServiceMock.Setup(x => x.LoginAsync(command.Name, command.Password, It.IsAny<CancellationToken>()))
				.ReturnsAsync(expectedResponse);

			// Act
			await _consumer.Consume(_contextMock.Object);

			// Assert
			_contextMock.Verify(x => x.RespondAsync(expectedResponse), Times.Once);
			_userServiceMock.Verify(x => x.LoginAsync(command.Name, command.Password, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task Consume_WithInvalidPassword_ShouldRespondWithFailure()
		{
			// Arrange
			var command = new LoginUserCommand
			{
				Name = "john",
				Password = "wrongpass"
			};

			var expectedResponse = new LoginResponse
			{
				IsSuccessful = false
			};

			_contextMock.Setup(x => x.Message).Returns(command);
			_userServiceMock.Setup(x => x.LoginAsync(command.Name, command.Password, It.IsAny<CancellationToken>()))
				.ReturnsAsync(expectedResponse);

			// Act
			await _consumer.Consume(_contextMock.Object);

			// Assert
			_contextMock.Verify(x => x.RespondAsync(It.Is<LoginResponse>(r =>
				!r.IsSuccessful)), Times.Once);
		}

		[Fact]
		public async Task Consume_WithNonExistentUser_ShouldRespondWithFailure()
		{
			// Arrange
			var command = new LoginUserCommand
			{
				Name = "unknown",
				Password = "pass123"
			};

			var expectedResponse = new LoginResponse
			{
				IsSuccessful = false
			};

			_contextMock.Setup(x => x.Message).Returns(command);
			_userServiceMock.Setup(x => x.LoginAsync(command.Name, command.Password, It.IsAny<CancellationToken>()))
				.ReturnsAsync(expectedResponse);

			// Act
			await _consumer.Consume(_contextMock.Object);

			// Assert
			_contextMock.Verify(x => x.RespondAsync(It.Is<LoginResponse>(r =>
				!r.IsSuccessful)), Times.Once);
		}
	}
}
