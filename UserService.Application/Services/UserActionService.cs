using Microsoft.Extensions.Caching.Distributed;
using Shared.Queryes;
using System.IdentityModel.Tokens.Jwt;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Domain;

namespace UserService.Application.Services
{
	public class UserActionService : IUserActionService
	{
		private readonly IUserRepository _userRepository;
		private readonly IJwtGenerator _jwtGenerator;
		private readonly IDistributedCache _cache;
		


		public UserActionService(IUserRepository userRepository, 
								 IDistributedCache cache,
								 IJwtGenerator jwtGenerator)
		{
			_userRepository = userRepository;
			_jwtGenerator = jwtGenerator;
			_cache = cache;			
		}

		public async Task<LoginResponse> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
		{
			var user = await _userRepository.GetByNameAsync(username, cancellationToken);
			if (user == null || password != user.Password)
			{				
				throw new UnauthorizedException("Invalid username or password");
			}
				

			var token = _jwtGenerator.GenerateToken(user);

			return new LoginResponse
			{
				IsSuccessful = true,
				Name = user.Name,
				Token = token
			};
		}

		public async Task<LogoutResponse> LogoutAsync(int id, string token, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(token))			{
				
				return new LogoutResponse
				{
					Success = false,
					Error = "Token is required"
				};
			}
			var tokenExpiration = GetTokenExpiration(token);

			if (tokenExpiration <= DateTime.UtcNow)
			{
				return new LogoutResponse
				{
					Success = false,
					Error = "Token already expired"
				};
			}

			await _cache.SetStringAsync(
					$"blacklist:{token}",
					$"revoked:{DateTime.UtcNow}",
					new DistributedCacheEntryOptions
					{
						AbsoluteExpiration = tokenExpiration
					},
					cancellationToken);

			return new LogoutResponse
			{
				Success = true				
			};			
		}

		public async Task<RegisterUserResponse> RegisterUserAsync(string username, string password)
		{
			var existingUser = await _userRepository.GetByNameAsync(username);

			if (existingUser != null) return new RegisterUserResponse() { Success = false, Error = "User wit this login already exists" };
			var user = new User(username, password);
			bool succsefullyAdded = await _userRepository.AddAsync(user);

			return new RegisterUserResponse() { Success = succsefullyAdded };
		}

		private DateTime GetTokenExpiration(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			try
			{					
				var jwtToken = handler.ReadJwtToken(token);								
				return jwtToken.ValidTo;
			}
			catch (Exception ex)
			{
				return DateTime.UtcNow.AddHours(1);
			}
		}
	}
}
