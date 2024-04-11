using WeatherApp.Models.OpenWeatherMapModels;

namespace WeatherApp.Services.Interfaces
{
    public interface IForecastRepository
    {
        WeatherResponse GetForecast(string city, string system);
    }
}
