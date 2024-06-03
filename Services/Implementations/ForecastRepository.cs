using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using WeatherApp.Models.OpenWeatherMapModels;
using WeatherApp.Services.Interfaces;
using WeatherApp.Config;

public class CityNotFoundException : Exception
{
    public CityNotFoundException(string message) : base(message)
    {
    }
}

public class ForecastRepository : IForecastRepository
{
    private readonly ILogger<ForecastRepository> _logger;

    public ForecastRepository(ILogger<ForecastRepository> logger)
    {
        _logger = logger;
    }

    public WeatherResponse GetForecast(string city, string system = null)
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
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new CityNotFoundException($"The city '{city}' does not exist!");
            }
            else
            {
                _logger.LogError($"Failed to retrieve forecast data. Status code: {response.StatusCode}, Message: {response.ErrorMessage}");
                // Throw another exception or handle the error as needed
                throw new Exception("Failed to retrieve forecast data.");
            }
        }
    }
}
