using MassTransit;
using Moq;
using Shared.Commands;
using Shared.Queryes;
using UserService.Application.Interfaces;
using UserService.Consumers;

namespace UserServiceTests
{
	public class RegisterUserConsumerTests
	{
		private readonly Mock<IUserActionService> _userServiceMock;
		private readonly RegisterUserConsumer _consumer;
		private readonly Mock<ConsumeContext<RegisterUserCommand>> _contextMock;

		public RegisterUserConsumerTests()
		{
			_userServiceMock = new Mock<IUserActionService>();			
			_consumer = new RegisterUserConsumer(_userServiceMock.Object);
			_contextMock = new Mock<ConsumeContext<RegisterUserCommand>>();
		}

		[Fact]
		public async Task Consume_ValidCommand_ShouldRespondWithSuccess()
		{
			// Arrange
			var command = new RegisterUserCommand
			{
				Name = "newuser",
				Password = "password123"
			};

			var expectedResponse = new RegisterUserResponse
			{
				Success = true,
				Error = null
			};

			_contextMock.Setup(x => x.Message).Returns(command);
			_userServiceMock.Setup(x => x.RegisterUserAsync(command.Name, command.Password))
				.ReturnsAsync(expectedResponse);

			// Act
			await _consumer.Consume(_contextMock.Object);

			// Assert
			_contextMock.Verify(x => x.RespondAsync(expectedResponse), Times.Once);
			_userServiceMock.Verify(x => x.RegisterUserAsync(command.Name, command.Password), Times.Once);
		}

		[Fact]
		public async Task Consume_WhenUserAlreadyExists_ShouldRespondWithError()
		{
			// Arrange
			var command = new RegisterUserCommand
			{
				Name = "existinguser",
				Password = "password123"
			};

			var expectedResponse = new RegisterUserResponse
			{
				Success = false,
				Error = "User with this name already exists"
			};

			_contextMock.Setup(x => x.Message).Returns(command);
			_userServiceMock.Setup(x => x.RegisterUserAsync(command.Name, command.Password))
				.ReturnsAsync(expectedResponse);

			// Act
			await _consumer.Consume(_contextMock.Object);

			// Assert
			_contextMock.Verify(x => x.RespondAsync(It.Is<RegisterUserResponse>(r =>
				!r.Success &&
				r.Error == expectedResponse.Error)), Times.Once);
		}

		[Fact]
		public async Task Consume_WhenPasswordTooShort_ShouldRespondWithError()
		{
			// Arrange
			var command = new RegisterUserCommand
			{
				Name = "newuser",
				Password = "123" // слишком короткий
			};

			var expectedResponse = new RegisterUserResponse
			{
				Success = false,
				Error = "Password must be at least 6 characters"
			};

			_contextMock.Setup(x => x.Message).Returns(command);
			_userServiceMock.Setup(x => x.RegisterUserAsync(command.Name, command.Password))
				.ReturnsAsync(expectedResponse);

			// Act
			await _consumer.Consume(_contextMock.Object);

			// Assert
			_contextMock.Verify(x => x.RespondAsync(It.Is<RegisterUserResponse>(r =>
				!r.Success &&
				r.Error == expectedResponse.Error)), Times.Once);
		}

		[Fact]
		public async Task Consume_WhenUsernameIsEmpty_ShouldRespondWithError()
		{
			// Arrange
			var command = new RegisterUserCommand
			{
				Name = "",
				Password = "password123"
			};

			var expectedResponse = new RegisterUserResponse
			{
				Success = false,
				Error = "Username cannot be empty"
			};

			_contextMock.Setup(x => x.Message).Returns(command);
			_userServiceMock.Setup(x => x.RegisterUserAsync(command.Name, command.Password))
				.ReturnsAsync(expectedResponse);

			// Act
			await _consumer.Consume(_contextMock.Object);

			// Assert
			_contextMock.Verify(x => x.RespondAsync(It.Is<RegisterUserResponse>(r =>
				!r.Success &&
				r.Error == expectedResponse.Error)), Times.Once);
		}

		[Fact]
		public async Task Consume_WhenServiceThrowsException_ShouldRespondWithError()
		{
			// Arrange
			var command = new RegisterUserCommand
			{
				Name = "newuser",
				Password = "password123"
			};

			var expectedResponse = new RegisterUserResponse
			{
				Success = false,
				Error = "Internal server error"
			};

			_contextMock.Setup(x => x.Message).Returns(command);
			_userServiceMock.Setup(x => x.RegisterUserAsync(command.Name, command.Password))
				.ReturnsAsync(expectedResponse);

			// Act
			await _consumer.Consume(_contextMock.Object);

			// Assert
			_contextMock.Verify(x => x.RespondAsync(It.Is<RegisterUserResponse>(r =>
				!r.Success &&
				r.Error == expectedResponse.Error)), Times.Once);
		}

		[Fact]
		public async Task Consume_WithInvalidCommand_ShouldStillCallService()
		{
			// Arrange
			var command = new RegisterUserCommand
			{
				Name = null,
				Password = null
			};

			var expectedResponse = new RegisterUserResponse
			{
				Success = false,
				Error = "Invalid input"
			};

			_contextMock.Setup(x => x.Message).Returns(command);
			_userServiceMock.Setup(x => x.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(expectedResponse);

			// Act
			await _consumer.Consume(_contextMock.Object);

			// Assert
			_userServiceMock.Verify(x => x.RegisterUserAsync(command.Name, command.Password), Times.Once);
			_contextMock.Verify(x => x.RespondAsync(It.IsAny<RegisterUserResponse>()), Times.Once);
		}
	}
}
