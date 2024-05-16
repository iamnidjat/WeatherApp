using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models
{
    public class FavouriteLocation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        // Foreign key property
        [Required]
      //  [ForeignKey]
        public int UserId { get; set; }

        // Navigation property
        public User? User { get; set; }
    }
}
