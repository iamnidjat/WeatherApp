using WeatherApp.Models;

namespace WeatherApp.ViewModels
{
    public class CompositeAirQualitySearchModel
    {
        public AirQualityDTO? AirQualityDTO { get; set; }

        public NotFoundModel? NotFoundModel { get; set; }
    }
}
