namespace WeatherApp.Services.Interfaces
{
    public interface IWeatherAlertService
    {
        Task CheckWeatherAndSendAlerts(string location, string email);
    }
}
