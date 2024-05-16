using System.Reflection;
using WeatherApp.Models;
using WeatherApp.Models.OpenMeteoModels;
using WeatherApp.Services.Implementations;

namespace WeatherApp.ViewModels
{
    public class CompositeViewModel
    {
        public SearchCity? SearchCityModel { get; set; }

        public System? SystemModel { get; set; }

        public List<string>? SearchedCities { get; set; }
    }
}
