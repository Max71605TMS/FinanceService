using MassTransit;
using Shared.Commands;
using UserService.Application.Interfaces;

namespace UserService.Consumers
{
	public class RegisterUserConsumer : IConsumer<RegisterUserCommand>
	{
		private readonly IUserActionService _userService;

		public RegisterUserConsumer(IUserActionService userService)
		{
			_userService = userService;
		}

		public async Task Consume(ConsumeContext<RegisterUserCommand> context)
		{
			var result = await _userService.RegisterUserAsync(context.Message.Name, context.Message.Password);
			
			await context.RespondAsync(result);
		}
	}
}
