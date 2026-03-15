
namespace Shared.Queryes
{
	public record LoginResponse
	{
		public string Name { get; init; }
		public string Token { get; init; }
		public bool IsSuccessful { get; init; }
	}
}
