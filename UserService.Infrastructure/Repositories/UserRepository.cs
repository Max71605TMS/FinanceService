using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces;
using UserService.Domain;

namespace UserService.Infrastructure.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly UserDbContext _context;

		public UserRepository(UserDbContext context)
		{
			_context = context;
		}
		public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await _context.Users
				.AsNoTracking()
				.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
		}

		public async Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
		{
			return await _context.Users
				.AsNoTracking()
				.FirstOrDefaultAsync(u => u.Name == name, cancellationToken);
		}

		public async Task<bool> AddAsync(User user, CancellationToken cancellationToken = default)
		{
			await _context.Users.AddAsync(user, cancellationToken);
			var result = await _context.SaveChangesAsync(cancellationToken);

			if (result > 0)
			{				
				_context.Entry(user).State = EntityState.Detached;
			}

			return result > 0;
		}
	}
}
