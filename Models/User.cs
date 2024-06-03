using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WeatherApp.Models
{
    [Index("UserName", IsUnique = true)]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public string? Email { get; set; }

        public bool IsSubscribed { get; set; }

        public string? SubscribedCity { get; set; }

        // Navigation property
        public ICollection<FavouriteLocation>? FavouriteLocations { get; set; }
    }
}
