using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;

namespace ApiGateway.Middleware
{
	public class TokenBlacklistMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IDistributedCache _cache;
		private readonly ILogger<TokenBlacklistMiddleware> _logger;

		public TokenBlacklistMiddleware(
			RequestDelegate next,
			IDistributedCache cache,
			ILogger<TokenBlacklistMiddleware> logger)
		{
			_next = next;
			_cache = cache;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var endpoint = context.GetEndpoint();
						
			if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
			{
				await _next(context);
				return;
			}

			var token = context.Request.Headers["Authorization"]
				.ToString()
				?.Replace("Bearer ", "");

			if (!string.IsNullOrEmpty(token))
			{
				var blacklisted = await _cache.GetStringAsync($"blacklist:{token}");
				if (!string.IsNullOrEmpty(blacklisted))
				{
					_logger.LogWarning("Attempt to use blacklisted token");
					context.Response.StatusCode = StatusCodes.Status401Unauthorized;
					await context.Response.WriteAsJsonAsync(new { error = "Token has been revoked" });
					return;
				}
			}

			await _next(context);
		}
	}
}
