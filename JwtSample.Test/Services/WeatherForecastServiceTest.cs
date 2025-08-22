using JwtSample.Services;

namespace JwtSample.Test.Services;

public class WeatherForecastServiceTest
{
	private readonly WeatherForecastService _service = new();
	
    [Fact]
    public void When_Request_Forecast_GetList_of_Forecast()
    {
		// Arrange:
		
		// Act:
		var forecast = _service.GetWeatherForecast();
		
		// Assert:
		Assert.NotNull(forecast);
    }
}
