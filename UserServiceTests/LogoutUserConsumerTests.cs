using MassTransit;
using Moq;
using Shared.Commands;
using Shared.Queryes;
using UserService.Application.Interfaces;
using UserService.Consumers;

namespace UserServiceTests
{
	public class LogoutUserConsumerTests
	{
		private readonly Mock<IUserActionService> _userServiceMock;
		private readonly LogoutUserConsumer _consumer;
		private readonly Mock<ConsumeContext<LogoutUserCommand>> _contextMock;

		public LogoutUserConsumerTests()
		{
			_userServiceMock = new Mock<IUserActionService>();			
			_consumer = new LogoutUserConsumer(_userServiceMock.Object);
			_contextMock = new Mock<ConsumeContext<LogoutUserCommand>>();
		}

		[Fact]
		public async Task Consume_WithValidData_ShouldRespondWithSuccess()
		{
			// Arrange
			var command = new LogoutUserCommand
			{
				UserId = 1,
				Token = "jwt_token"
			};

			var expectedResponse = new LogoutResponse
			{
				Success = true
			};

			_contextMock.Setup(x => x.Message).Returns(command);
			_userServiceMock.Setup(x => x.LogoutAsync(command.UserId, command.Token, It.IsAny<CancellationToken>()))
				.ReturnsAsync(expectedResponse);

			// Act
			await _consumer.Consume(_contextMock.Object);

			// Assert
			_contextMock.Verify(x => x.RespondAsync(expectedResponse), Times.Once);
			_userServiceMock.Verify(x => x.LogoutAsync(command.UserId, command.Token, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task Consume_WithInvalidUser_ShouldRespondWithFailure()
		{
			// Arrange
			var command = new LogoutUserCommand
			{
				UserId = 999,
				Token = "jwt_token"
			};

			var expectedResponse = new LogoutResponse
			{
				Success = false,
				Error = "User not found"
			};

			_contextMock.Setup(x => x.Message).Returns(command);
			_userServiceMock.Setup(x => x.LogoutAsync(command.UserId, command.Token, It.IsAny<CancellationToken>()))
				.ReturnsAsync(expectedResponse);

			// Act
			await _consumer.Consume(_contextMock.Object);

			// Assert
			_contextMock.Verify(x => x.RespondAsync(It.Is<LogoutResponse>(r =>
				!r.Success && r.Error == expectedResponse.Error)), Times.Once);
		}

		[Fact]
		public async Task Consume_WithEmptyToken_ShouldRespondWithFailure()
		{
			// Arrange
			var command = new LogoutUserCommand
			{
				UserId = 1,
				Token = ""
			};

			var expectedResponse = new LogoutResponse
			{
				Success = false,
				Error = "Token is required"
			};

			_contextMock.Setup(x => x.Message).Returns(command);
			_userServiceMock.Setup(x => x.LogoutAsync(command.UserId, command.Token, It.IsAny<CancellationToken>()))
				.ReturnsAsync(expectedResponse);

			// Act
			await _consumer.Consume(_contextMock.Object);

			// Assert
			_contextMock.Verify(x => x.RespondAsync(It.Is<LogoutResponse>(r =>
				!r.Success)), Times.Once);
		}
	}
}
