using MassTransit;
using Shared.Commands;
using UserService.Application.Interfaces;

namespace UserService.Consumers
{
	public class LogoutUserConsumer : IConsumer<LogoutUserCommand>
	{
		private readonly IUserActionService _service;

		public LogoutUserConsumer(IUserActionService service)
		{
			_service = service;
		}
		public async Task Consume(ConsumeContext<LogoutUserCommand> context)
		{
			var result = await _service.LogoutAsync(context.Message.UserId, context.Message.Token);

			await context.RespondAsync(result);
		}
	}
}
