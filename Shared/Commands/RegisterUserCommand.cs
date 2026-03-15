namespace Shared.Commands
{
	public record RegisterUserCommand
	{
		public string Name { get; init; }
		public string Password { get; init; }
	}
}
