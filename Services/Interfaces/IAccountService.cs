using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Services.Interfaces
{
    public interface IAccountService
    {
        Task<int> ChangePasswordAsync(string login, string oldPassword, string newPassword);
        Task<int> ChangeEmailAsync(string login, string email);
    }
}
