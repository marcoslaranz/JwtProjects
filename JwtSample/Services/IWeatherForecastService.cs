using JwtSample.Models;

namespace JwtSample.Services;

public interface IWeatherForecastService
{
	public IEnumerable<WeatherForecast> GetWeatherForecast();
}