using System.Reflection;
using WeatherApp.Models;
using WeatherApp.Models.OpenMeteoModels;
using WeatherApp.Services.Implementations;
using static WeatherApp.Services.Implementations.GetAdditionalDataRepository;

namespace WeatherApp.ViewModels
{
    public class CompositeViewModel
    {
        public SearchCity? SearchCityModel { get; set; }

        public System? SystemModel { get; set; }

        public ForecastDataRequest? ForecastDataRequest { get; set; }

        public AirQualityDataRequest? AirQualityDataRequest { get; set; }

        public SunriseSunsetModel? SunriseSunsetModel { get; set; }

        public List<SearchedCities>? SearchedCities { get; set; }
    }
}
