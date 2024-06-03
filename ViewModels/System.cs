using System.ComponentModel.DataAnnotations;

namespace WeatherApp.ViewModels
{
    public class System
    {
        [Required(ErrorMessage = "You must specify a system!")]
        public string? Units { get; set; }
    }
}
