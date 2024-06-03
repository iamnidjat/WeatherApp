using System.ComponentModel.DataAnnotations;

namespace WeatherApp.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Username is not specified")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is not specified")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email is not specified")]
        [EmailAddress]
        public string? Email { get; set; }

        public bool RememberMe { get; set; }
    }
}
