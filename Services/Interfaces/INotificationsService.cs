using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Services.Interfaces
{
    public interface INotificationsService
    {
        Task<IActionResult> SendWeatherAlertAsync(string email, string city, string message);
    }
}
