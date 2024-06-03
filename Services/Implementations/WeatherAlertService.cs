using WeatherApp.Enums;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services.Implementations
{
    public class WeatherAlertService : IWeatherAlertService
    {
        private readonly IWeatherService _weatherService;
        private readonly INotificationsService _notificationService;

        public WeatherAlertService(IWeatherService weatherService, INotificationsService notificationService)
        {
            _weatherService = weatherService;
            _notificationService = notificationService;
        }

        // Method to check weather conditions and send alerts
        public async Task CheckWeatherAndSendAlerts(string location, string email)
        {
            // Retrieve weather forecast for the location
            var weatherForecast = _weatherService.GetWeatherCondition(location);

            // Check if weather forecast indicates bad weather conditions
            if (IsBadWeather(weatherForecast))
            {
                // Send email notification
                await _notificationService.SendWeatherAlertAsync(email, location, weatherForecast);
            }
        }

        // Method to determine if weather conditions are bad
        private bool IsBadWeather(string forecastCondition)
        {
            return forecastCondition == WeatherCondition.Rain.ToString() ||
                   forecastCondition == WeatherCondition.Drizzle.ToString() ||
                   forecastCondition == WeatherCondition.Thunderstorm.ToString() ||
                   forecastCondition == WeatherCondition.Snow.ToString() ||
                   forecastCondition == WeatherCondition.Mist.ToString() ||
                   forecastCondition == WeatherCondition.Smoke.ToString() ||
                   forecastCondition == WeatherCondition.Dust.ToString() ||
                   forecastCondition == WeatherCondition.Fog.ToString() ||
                   forecastCondition == WeatherCondition.Sand.ToString() ||
                   forecastCondition == WeatherCondition.Ash.ToString() ||
                   forecastCondition == WeatherCondition.Squall.ToString() ||
                   forecastCondition == WeatherCondition.Tornado.ToString() ||
                   forecastCondition == WeatherCondition.Haze.ToString();
        }
    }
}
