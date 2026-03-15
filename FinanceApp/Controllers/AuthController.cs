using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using Shared.Queryes;
using Shared.Commands;
using RegisterRequest = Shared.Queryes.RegisterRequest;
using LoginRequest = Shared.Commands.LoginRequest;

namespace ApiGateway.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous]
	public class AuthController : BaseGatewayController
	{
		private readonly IRequestClient<RegisterUserCommand> _registerClient;
		private readonly IRequestClient<LoginUserCommand> _loginClient;
		private readonly IRequestClient<LogoutUserCommand> _logoutClient;

		public AuthController(
		IRequestClient<RegisterUserCommand> registerClient,
		IRequestClient<LoginUserCommand> loginClient,
		IRequestClient<LogoutUserCommand> logoutClient
		)
		{
			_registerClient = registerClient;
			_loginClient = loginClient;
			_logoutClient = logoutClient;
		}

		/// <summary>
		/// Регистрация нового пользователя
		/// </summary>
		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterRequest request)
		{
			var command = new RegisterUserCommand
			{
				Name = request.Name,
				Password = request.Password
			};

			var response = await _registerClient.GetResponse<RegisterUserResponse>(command);

			if (!response.Message.Success)
			{
				return BadRequest(new { error = response.Message.Error ?? "Registration failed" });
			}
		
			return Ok(response.Message);
		}

		/// <summary>
		/// Вход пользователя
		/// </summary>
		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			var command = new LoginUserCommand
			{
				Name = request.Name,
				Password = request.Password
			};

			var response = await _loginClient.GetResponse<LoginResponse>(command);
			if (!response.Message.IsSuccessful)
			{
				return Unauthorized(new { error = response.Message });				
			}

			return Ok(response.Message);
		}

		/// <summary>
		/// Выход из системы
		/// </summary>
		[Authorize]
		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			var userId = GetUserId();

			if (userId is null)
				return Unauthorized(new { error = "User not authenticated" });

			var token = HttpContext.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

			if (string.IsNullOrEmpty(token))
				return BadRequest(new { error = "No token provided" });

			var command = new LogoutUserCommand
			{
				UserId = userId.Value,
				Token = token
			};

			var response = await _logoutClient.GetResponse<LogoutResponse>(command);
			
			if (!response.Message.Success)
				return BadRequest(new { error = "Logout failed" });

			return Ok(new { message = "Logged out successfully" });
		}
	}
}
