using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using WeatherApp.Models.SunriseSunsetModels;
using WeatherApp.Services.Interfaces;
using System.Globalization;

namespace WeatherApp.Services.Implementations
{
    public class GetAdditionalDataRepository : IGetAdditionalDataRepository
    {
        private readonly ILogger<GetAdditionalDataRepository> _logger;

        public GetAdditionalDataRepository(ILogger<GetAdditionalDataRepository> logger)
        {
            _logger = logger;
        }

        public Root GetAdditionalData(SunriseSunsetModel sunriseSunsetModel)
        {
            // Correctly format latitude and longitude to use periods as decimal separators
            var latitude = sunriseSunsetModel.lat.ToString(CultureInfo.InvariantCulture);
            var longitude = sunriseSunsetModel.lng.ToString(CultureInfo.InvariantCulture);

            string apiUrl = $"https://api.sunrisesunset.io/json?lat={latitude}&lng={longitude}";

            if (!string.IsNullOrEmpty(sunriseSunsetModel.date))
                apiUrl += $"&date={sunriseSunsetModel.date}";

            var client = new RestClient(apiUrl);
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                var content = JsonConvert.DeserializeObject<JToken>(response.Content);
                return content.ToObject<Root>();
            }
            else
            {
                _logger.LogError($"Failed to retrieve sunrise/sunset data. Status code: {response.StatusCode}, Message: {response.ErrorMessage}");
                throw null;
            }
        }

        public record SunriseSunsetModel(float lat, float lng, string date = null);
    }
}

