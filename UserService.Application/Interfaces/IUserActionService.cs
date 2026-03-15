using Shared.Queryes;
namespace UserService.Application.Interfaces
{
	public interface IUserActionService
	{
		Task<RegisterUserResponse> RegisterUserAsync(string username, string password);
		Task<LoginResponse> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
		Task<LogoutResponse> LogoutAsync (int id, string token, CancellationToken cancellationToken = default);
	}
}
