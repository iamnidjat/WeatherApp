using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using WeatherApp.Config;
using WeatherApp.Models.OpenWeatherMapModels;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services.Implementations
{
    public class ForecastRepository : IForecastRepository
    {
        public WeatherResponse GetForecast(string city, string system)
        {
            string openWeatherApiKey = Constants.OPEN_WEATHER_APPID;
            string apiUrl = $"http://api.openweathermap.org/data/2.5/weather?q={city}&units={system}&APPID={openWeatherApiKey}";

            var client = new RestClient(apiUrl);
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                var content = JsonConvert.DeserializeObject<JToken>(response.Content);
                return content.ToObject<WeatherResponse>();
            }
            else
            {
                // Handle error appropriately, throw an exception or return a default response
                throw new Exception($"Failed to retrieve forecast data. Status code: {response.StatusCode}, Message: {response.ErrorMessage}");
            }
        }
    }
}
