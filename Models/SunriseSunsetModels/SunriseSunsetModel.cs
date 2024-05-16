using Newtonsoft.Json;

namespace WeatherApp.Models.SunriseSunsetModels
{
    public class Results
    {
        public string date { get; set; }
        public string sunrise { get; set; }
        public string sunset { get; set; }
        public object first_light { get; set; }
        public object last_light { get; set; }
        public string dawn { get; set; }
        public string dusk { get; set; }
        public string solar_noon { get; set; }
        public string golden_hour { get; set; }
        public string day_length { get; set; }
        public string timezone { get; set; }
        public int utc_offset { get; set; }
    }

    public class Root
    {
        public Results results { get; set; }
        public string status { get; set; }
    }
}
