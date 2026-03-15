namespace Shared.Commands
{
	public record LogoutUserCommand
	{
		public int UserId { get; init; }
		public string Token { get; init; }
	}
}
