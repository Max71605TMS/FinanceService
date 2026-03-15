using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiGateway.Controllers
{
	[ApiController]
	public abstract class BaseGatewayController : ControllerBase
	{
		protected int? GetUserId()
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userIdClaim)) return null;

			return int.Parse(userIdClaim);
		}
	}
}
