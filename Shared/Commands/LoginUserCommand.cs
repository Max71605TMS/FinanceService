namespace Shared.Commands
{
	public record LoginUserCommand
	{
		public string Name { get; init; }
		public string Password { get; init; }
	}
}
