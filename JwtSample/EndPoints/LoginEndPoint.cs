using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using JwtSample.Services;
using JwtSample.Models;
using JwtSample.DTOs;
using JwtSample.Repositories;
using JwtSample.Entities;
using Microsoft.Extensions.Logging;

namespace JwtSample.EndPoints;

public static class LoginEndPoint
{
	public static WebApplication MapLogin(this WebApplication app)
	{
		app.MapPost("/auth/login", async (LoginRequestModel request, IJwtService jwt, ILogger<IJwtService> _logger) =>
		{
			var result = await jwt.Authenticate(request);
			
			_logger.LogInformation($"/auth/login: request = {request.Username} {request.Password}");
			
			return result is not null ? Results.Json(result) : Results.Unauthorized();
		});
		
		app.MapPost("/auth/refresh", async (RefreshRequestModel request, IJwtService jwt, ILogger<IJwtService> _logger) =>
		{
			_logger.LogInformation($"/auth/refresh: request = {request.Token}");
			
			if(string.IsNullOrWhiteSpace(request.Token))
			{
				return Results.BadRequest("Invalid Token!");
			}
			
			var result = await jwt.ValidateRefreshToken(request.Token);
			
			return result is not null ? Results.Json(result): Results.Unauthorized();
		});
		return app;
	}
}
