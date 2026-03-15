using UserService.Domain;

namespace UserService.Application.Interfaces
{
	public interface IUserRepository
	{
		Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
		Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
		Task<bool> AddAsync(User user, CancellationToken cancellationToken = default);
	}
}
