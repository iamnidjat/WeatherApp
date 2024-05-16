using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using WeatherApp.Models.SunriseSunsetModels;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services.Implementations
{
    public class GetAdditionalDataRepository: IGetAdditionalDataRepository
    {
        private readonly ILogger<GetAdditionalDataRepository> _logger;

        public GetAdditionalDataRepository(ILogger<GetAdditionalDataRepository> logger)
        {
            _logger = logger;
        }

        public Root GetAdditionalData(decimal lat, decimal lng, string timezone = null, string date = null, string startDate = null, string endDate = null)
        {
            string apiUrl = $"https://api.sunrisesunset.io/json?lat={lat}&lng={lng}";

            if (!string.IsNullOrEmpty(timezone))
                apiUrl += $"&timezone={timezone}";
            if (!string.IsNullOrEmpty(date))
                apiUrl += $"&date={date}";
            if (!string.IsNullOrEmpty(startDate))
                apiUrl += $"&date_start={startDate}";
            if (!string.IsNullOrEmpty(endDate))
                apiUrl += $"&date_end={endDate}";

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
    }
}

