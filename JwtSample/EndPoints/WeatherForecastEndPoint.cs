using JwtSample.Models;
using JwtSample.Services;

namespace JwtSample.EndPoints;

public static class WeatherForecastEndPoint
{
	public static WebApplication MapWeatherForecast(this WebApplication app)
	{
		app.MapGet("/weatherforecast", (IWeatherForecastService service) => 
		{
			return service.GetWeatherForecast();
		})
		.RequireAuthorization();
		// This ensures authentication is required
		// .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
		// This will work with the Token, see GenerateJwtToken in JwtService.
		// Or if you define in your Program.cs an specific Autorization like
		// this:
		//builder.Services.AddAuthorization(options =>
		//{
		//    options.AddPolicy("AdminOnly", policy =>
		//        policy.RequireRole("Admin"));
		//});
		// Then you coud use this at end of your endpoint:
		// .RequireAuthorization("AdminOnly");


		return app;
	}
}