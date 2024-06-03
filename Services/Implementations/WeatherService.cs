using Newtonsoft.Json;
using RestSharp;
using WeatherApp.Models.OpenWeatherMapModels;
using WeatherApp.Services.Interfaces;
using WeatherApp.Config;

namespace WeatherApp.Services.Implementations
{
    public class WeatherService : IWeatherService
    {
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(ILogger<WeatherService> logger)
        {
            _logger = logger;
        }

        public string GetWeatherCondition(string city)
        {
            string openWeatherApiKey = Constants.OPEN_WEATHER_APPID;
            string apiUrl = $"http://api.openweathermap.org/data/2.5/weather?q={city}&APPID={openWeatherApiKey}";

            var client = new RestClient(apiUrl);
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                var content = JsonConvert.DeserializeObject<WeatherResponse>(response.Content);
                return content.Weather.FirstOrDefault()?.Main;
            }

            return "";
        }
    }
}
