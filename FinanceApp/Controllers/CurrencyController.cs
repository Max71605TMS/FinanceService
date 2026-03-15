using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using Shared.Queryes;

namespace ApiGateway.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CurrencyController : BaseGatewayController
	{
		private readonly IRequestClient<RatesForUserQuery> _ratesClient;

		public CurrencyController(
		IRequestClient<RatesForUserQuery> ratesClient)
		{
			_ratesClient = ratesClient;
		}

		/// <summary>
		/// Получить курсы валют пользователя
		/// </summary>
		[HttpGet("rates")]
		public async Task<IActionResult> GetUserRates()
		{
			var userId = GetUserId();
			if (userId == null) return Unauthorized(new { error = "User not authenticated" });

			var query = new RatesForUserQuery { UserId = userId.Value };
			var response = await _ratesClient.GetResponse<CurrencyRatesResponse>(query);

			return Ok(response.Message.Rates);
		}
	}
}
