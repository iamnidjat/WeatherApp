using WeatherApp.Models.SunriseSunsetModels;

namespace WeatherApp.Services.Interfaces
{
    public interface IGetAdditionalDataRepository
    {
        Root GetAdditionalData(decimal lat, decimal lng, string timezone = null, string date = null, string startDate = null, string endDate = null);
    }
}
