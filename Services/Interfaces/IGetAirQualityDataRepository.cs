using WeatherApp.Models.OpenMeteoModels;
using WeatherApp.Services.Implementations;

namespace WeatherApp.Services.Interfaces
{
    public interface IGetAirQualityDataRepository
    {
        AirQualityModel GetAirQualityData(AirQualityDataRequest request);
    }
}
