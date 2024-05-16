using WeatherApp.Models;

namespace WeatherApp.ViewModels
{
    public class CompositeSearchModel
    {
        public City? City { get; set; }

        public NotFoundModel? NotFoundModel { get; set; }
    }
}
