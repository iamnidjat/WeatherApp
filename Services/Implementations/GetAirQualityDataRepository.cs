using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using WeatherApp.Models.OpenMeteoModels;
using WeatherApp.Services.Interfaces;
using WeatherApp.Models;
using System.Globalization;

namespace WeatherApp.Services.Implementations
{
    public class GetAirQualityDataRepository : IGetAirQualityDataRepository
    {
        private readonly ILogger<GetAirQualityDataRepository> _logger;

        public GetAirQualityDataRepository(ILogger<GetAirQualityDataRepository> logger)
        {
            _logger = logger;
        }

        public AirQualityModel GetAirQualityData(AirQualityDataRequest request)
        {
            // Correctly format latitude and longitude to use periods as decimal separators
            var latitude = request.Latitude.ToString(CultureInfo.InvariantCulture);
            var longitude = request.Longitude.ToString(CultureInfo.InvariantCulture);

            string apiUrl = $"https://air-quality-api.open-meteo.com/v1/air-quality?latitude={latitude}&longitude={longitude}&hourly=pm10";

            apiUrl += AppendParameter("past_days", request.PastDays.ToString());
            apiUrl += AppendParameter("forecast_days", request.ForecastDays.ToString());
            apiUrl += AppendParameter("start_date", request.StartDate);
            apiUrl += AppendParameter("end_date", request.EndDate);
            apiUrl += AppendParameter("start_hour", request.StartHour);
            apiUrl += AppendParameter("end_hour", request.EndHour);

            var client = new RestClient(apiUrl);
            var response = client.Execute(new RestRequest(Method.GET));

            if (response.IsSuccessful)
            {
                var content = JsonConvert.DeserializeObject<JToken>(response.Content);
                return content.ToObject<AirQualityModel>();
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new CityNotFoundException($"The city does not exist!");
                }
                else
                {
                    _logger.LogError($"Failed to retrieve air quality data. Status code: {response.StatusCode}, Message: {response.ErrorMessage}");
                    // Throw another exception or handle the error as needed
                    throw new Exception("Failed to retrieve air quality data.");
                }
                
            }
        }

        private string AppendParameter(string parameterName, string parameterValue)
        {
            return !string.IsNullOrEmpty(parameterValue) ? $"&{parameterName}={parameterValue}" : "";
        }
    }

    public record AirQualityDataRequest(float Latitude, float Longitude,
        int? PastDays = null, int? ForecastDays = null, string? StartDate = null, 
        string? EndDate = null, string? StartHour = null, string? EndHour = null);
}

