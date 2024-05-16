using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using WeatherApp.Models.SunriseSunsetModels;
using WeatherApp.Models.OpenMeteoModels;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services.Implementations
{
    public class GetAirQualityDataRepository: IGetAirQualityDataRepository
    {
        private readonly ILogger<GetAirQualityDataRepository> _logger;

        public GetAirQualityDataRepository(ILogger<GetAirQualityDataRepository> logger)
        {
            _logger = logger;
        }

        public AirQualityModel GetForecastData(AirQualityDataRequest request)
        {
            string apiUrl = $"https://air-quality-api.open-meteo.com/v1/air-quality?latitude={request.Latitude}&longitude={request.Longitude}";

            apiUrl += AppendParameter("hourly", request.HourlyData);
            apiUrl += AppendParameter("timezone", request.Timezone);
            apiUrl += AppendParameter("past_days", request.PastDays);
            apiUrl += AppendParameter("forecast_days", request.ForecastDays);
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
                _logger.LogError($"Failed to retrieve air quality data. Status code: {response.StatusCode}, Message: {response.ErrorMessage}");
                throw null;
            }
        }

        private string AppendParameter(string parameterName, string parameterValue)
        {
            return !string.IsNullOrEmpty(parameterValue) ? $"&{parameterName}={parameterValue}" : "";
        }
    }

    public record AirQualityDataRequest(decimal Latitude, decimal Longitude,
        string? HourlyData, string? Timezone, string? PastDays, 
        string? ForecastDays, string? StartDate, string? EndDate, 
        string? StartHour, string? EndHour);
}

