using System.ComponentModel.DataAnnotations;

namespace WeatherApp.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is not specified")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is not specified")]
        public string? Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
