using System.Reflection;
using WeatherApp.Models;

namespace WeatherApp.ViewModels
{
    public class CompositeViewModel
    {
        public SearchCity? SearchCityModel { get; set; }
        public System? SystemModel { get; set; }
        public List<string>? SearchedCities { get; set; }
    }
}
