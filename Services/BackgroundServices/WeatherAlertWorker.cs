using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services.BackgroundServices
{
    public class WeatherAlertWorker
    {
        private readonly IAccountService _accountService;
        private readonly IWeatherAlertService _weatherAlertService;

        public WeatherAlertWorker(IAccountService accountService, IWeatherAlertService weatherAlertService)
        {
            _accountService = accountService;
            _weatherAlertService = weatherAlertService;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            try
            {
                // Retrieve users and their subscribed locations from the database
                var subscriptions = await _accountService.GetSubscribedUsersAsync(); // Adjust the entity name

                foreach (var subscription in subscriptions)
                {
                    var location = subscription.SubscribedCity;
                    var email = subscription.Email;

                    // Check weather and send alerts for each subscription
                    await _weatherAlertService.CheckWeatherAndSendAlerts(location, email);
                }
            }
            catch (Exception ex)
            {
                // Log any unexpected exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

}
