namespace Shared.Queryes
{
	public record RegisterUserResponse
	{
		public bool Success {  get; init; }
		public string Error { get; init; }
	}
}
