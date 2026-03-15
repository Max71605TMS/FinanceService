using UserService.Domain;

namespace UserService.Application.Interfaces
{
	public interface IJwtGenerator
	{
		string GenerateToken(User user);
	}
}
