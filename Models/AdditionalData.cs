using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models
{
    public class AdditionalData
    {
        [Display(Name = "Date: ")]
        public string? Date { get; set; }

        [Display(Name = "Sunrise: ")]
        public string? Sunrise { get; set; }

        [Display(Name = "Sunset: ")]
        public string? Sunset { get; set; }

        [Display(Name = "FirstLight: ")]
        public object? FirstLight { get; set; }

        [Display(Name = "LastLight: ")]
        public object? LastLight { get; set; }

        [Display(Name = "Dawn: ")]
        public string? Dawn { get; set; }

        [Display(Name = "Dusk: ")]
        public string? Dusk { get; set; }

        [Display(Name = "SolarNoon: ")]
        public string? SolarNoon { get; set; }

        [Display(Name = "GoldenHour: ")]
        public string? GoldenHour { get; set; }

        [Display(Name = "DayLength: ")]
        public string? DayLength { get; set; }

        [Display(Name = "Timezone: ")]
        public string? Timezone { get; set; }
    }
}
