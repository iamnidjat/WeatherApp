namespace WeatherApp.Models.OpenMeteoModels
{
    public class PHourly
    {
        public List<string> time { get; set; }
        public List<double> temperature_2m { get; set; }
    }

    public class PHourlyUnits
    {
        public string time { get; set; }
        public string temperature_2m { get; set; }
    }

    public class PDaily
    {
        public List<string> time { get; set; }
        public List<double> temperature_2m_max { get; set; }
    }

    public class PDailyUnits
    {
        public string time { get; set; }
        public string temperature_2m_max { get; set; }
    }

    public class PreciseForecastData
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double generationtime_ms { get; set; }
        public int utc_offset_seconds { get; set; }
        public string timezone { get; set; }
        public string timezone_abbreviation { get; set; }
        public double elevation { get; set; }
        public PHourlyUnits hourly_units { get; set; }
        public PHourly hourly { get; set; }
        public PDailyUnits daily_units { get; set; }
        public PDaily daily { get; set; }
    }
}
