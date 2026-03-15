using ApiGateway.Middleware;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Commands;
using System.Text;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", policy =>
				{
					policy.AllowAnyOrigin()
						  .AllowAnyMethod()
						  .AllowAnyHeader();
				});
			});
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Title = "API Gateway",
					Version = "v1",
					Description = "API Gateway для микросервисов"
				});
				
				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.Http,  
					Scheme = "bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Введите JWT токен в формате: {ваш токен}"
				});
								
				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
							{
							Reference = new OpenApiReference
								{
									Type = ReferenceType.SecurityScheme,
									Id = "Bearer"
								}
							},
						Array.Empty<string>()
					}
				});
			});
			builder.Services.ConfigureSwaggerGen(options =>
			{
				options.CustomSchemaIds(type => type.FullName);
			});

			builder.Services.AddMassTransit(x =>
			{				
				x.AddRequestClient<RegisterUserCommand>();
				x.AddRequestClient<LoginUserCommand>();
				x.AddRequestClient<LogoutUserCommand>();

				x.UsingRabbitMq((context, cfg) =>
				{
					cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
					{
						h.Username(builder.Configuration["RabbitMQ:Username"]);
						h.Password(builder.Configuration["RabbitMQ:Password"]);
					});

					cfg.ConfigureEndpoints(context);
				});
			});

			builder.Services.AddStackExchangeRedisCache(options =>
			{
				options.Configuration = builder.Configuration.GetConnectionString("Redis");
				options.InstanceName = "TokenBlacklist";
			});

			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
		};

		options.Events = new JwtBearerEvents
		{
			OnAuthenticationFailed = context =>
			{
				var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
				logger.LogError("Authentication failed: {Message}", context.Exception.Message);
				return Task.CompletedTask;
			},
			OnTokenValidated = context =>
			{
				var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
				logger.LogDebug("Token validated for user: {User}",
					context.Principal?.Identity?.Name);
				return Task.CompletedTask;
			}
		};
	});
			var app = builder.Build();
						
			if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker")) 
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			if (!builder.Environment.IsEnvironment("Docker"))
			{
				app.UseHttpsRedirection();
			}
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway V1");
				c.DocumentTitle = "API Gateway Documentation";
				c.EnableDeepLinking();
				c.DisplayRequestDuration();
			});

			app.UseHttpsRedirection();
			app.UseCors("AllowAll");
			app.UseRouting();
			app.UseAuthorization();
			app.UseMiddleware<TokenBlacklistMiddleware>();

			app.MapControllers();

            app.Run();
        }
    }
}
