using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models
{
    public class City
    {
        [Display(Name = "City: ")]
        public string? Name { get; set; }

        [Display(Name = "Temperature: ")]
        public float Temp { get; set; }

        [Display(Name = "Min temperature: ")]
        public float MinTemp { get; set; }

        [Display(Name = "Max temperature: ")]
        public float MaxTemp { get; set; }

        [Display(Name = "Feels like: ")]
        public float FeelsLike { get; set; }

        [Display(Name = "Humidity: ")]
        public int Humidity { get; set; }

        [Display(Name = "Pressure: ")]
        public int Pressure { get; set; }

        [Display(Name = "Wind Speed: ")]
        public float Wind { get; set; }

        [Display(Name = "Weather condition: ")]
        public string? Weather { get; set; }

        [Display(Name = "Weather description: ")]
        public string? WeatherDescription { get; set; }

        [Display(Name = "Weather Icon: ")]
        public string? WeatherIcon { get; set; }

        [Display(Name = "Visibility: ")]
        public int Visibility { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

    }
}
