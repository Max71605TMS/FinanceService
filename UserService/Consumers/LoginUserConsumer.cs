using MassTransit;
using Shared.Commands;
using UserService.Application.Interfaces;

namespace UserService.Consumers
{
	public class LoginUserConsumer : IConsumer<LoginUserCommand>
	{
		private readonly IUserActionService _userService;
		private readonly ILogger<LoginUserConsumer> _logger;

		public LoginUserConsumer(IUserActionService userService, ILogger<LoginUserConsumer> logger)
		{
			_userService = userService;
			_logger = logger;
		}
		public async Task Consume(ConsumeContext<LoginUserCommand> context)
		{
			try
			{
				var result = await _userService.LoginAsync(context.Message.Name, context.Message.Password);
				await context.RespondAsync(result);
				_logger.LogInformation("User {UserName} logged in successfully", context.Message.Name);
			}
			catch (UnauthorizedAccessException)
			{
				_logger.LogWarning("Failed login attempt for user {UserName}", context.Message.Name);

				await context.RespondAsync(new
				{
					Error = "Invalid username or password"
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error logging in user {UserName}", context.Message.Name);

				await context.RespondAsync(new
				{
					Error = "An error occurred during login"
				});
			}			
		}
	}
}
