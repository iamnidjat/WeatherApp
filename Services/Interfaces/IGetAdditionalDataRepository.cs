using WeatherApp.Models.SunriseSunsetModels;
using static WeatherApp.Services.Implementations.GetAdditionalDataRepository;

namespace WeatherApp.Services.Interfaces
{
    public interface IGetAdditionalDataRepository
    {
        Root GetAdditionalData(SunriseSunsetModel sunriseSunsetModel);
    }
}
