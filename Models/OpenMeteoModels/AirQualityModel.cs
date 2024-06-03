namespace WeatherApp.Models.OpenMeteoModels
{
    public class Hourly
    {
        public List<string> time { get; set; }
        public List<double?> pm10 { get; set; }
    }

    public class HourlyUnits
    {
        public string time { get; set; }
        public string pm10 { get; set; }
    }

    public class AirQualityModel
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double generationtime_ms { get; set; }
        public int utc_offset_seconds { get; set; }
        public string timezone { get; set; }
        public string timezone_abbreviation { get; set; }
        public double elevation { get; set; }
        public HourlyUnits hourly_units { get; set; }
        public Hourly hourly { get; set; }
    }
}
