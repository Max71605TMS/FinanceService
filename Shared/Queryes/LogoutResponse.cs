namespace Shared.Queryes
{
	public record LogoutResponse
	{
		public bool Success { get; init; }

		public string Error { get; init; }
	}
}
