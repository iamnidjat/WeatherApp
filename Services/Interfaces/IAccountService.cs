using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces
{
    public interface IAccountService
    {
        Task<int> ChangePasswordAsync(string login, string oldPassword, string newPassword);
        Task<int> ChangeEmailAsync(string login, string email);
        Task<IEnumerable<User>> GetSubscribedUsersAsync();
        Task EnableSubscriptionAsync(int userId, string city);
        Task DisableSubscriptionAsync(int userId, string city);
        Task<IActionResult> ResetPasswordAsync(string email);
    }
}
