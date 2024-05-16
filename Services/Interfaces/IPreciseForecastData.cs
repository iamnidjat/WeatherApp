using WeatherApp.Models.OpenMeteoModels;
using WeatherApp.Services.Implementations;

namespace WeatherApp.Services.Interfaces
{
    public interface IPreciseForecastData
    {
        PreciseForecastData GetForecastData(ForecastDataRequest request);
    }
}
