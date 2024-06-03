using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models
{
    public class FullWeatherData
    {
        [Display(Name = "Times: ")]
        public List<string> Times { get; set; }

        [Display(Name = "Temperatures: ")]
        public List<double> Temperatures { get; set; }
    }
}
