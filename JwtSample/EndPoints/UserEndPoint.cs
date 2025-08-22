using JwtSample.Models;
using JwtSample.Services;
using JwtSample.DTOs;
using JwtSample.Mappings;
using Microsoft.Extensions.Logging;

namespace JwtSample.EndPoints;

public static class UserEndPoint
{
	public static WebApplication MapUser(this WebApplication app)
	{
		app.MapGet("/users", async (UserService service) =>
        {
           return await service.GetAllAsync();
        });
		
		
		app.MapGet("/users/{id:int}", async (int id, UserService service, ILogger<UserService> _logger) =>
        {
		   _logger.LogInformation($"GetUserById: {id}");
		   
           var user = await service.GetByIdAsync(id);
		   return user is not null ? Results.Ok(user) : Results.NotFound();
        })
		.WithName("GetUserById");
		
		
		app.MapPost("/users", async (LoginRequestModel userRequest, UserService service, ILogger<UserService> _logger) =>
        {
		   _logger.LogInformation("Get All Users");
		   
           var user = await service.AddAsync(userRequest);
		   
		   return Results.CreatedAtRoute("GetUserById", new { id = user.Id }, user );
        });
		
		return app;
	}
}