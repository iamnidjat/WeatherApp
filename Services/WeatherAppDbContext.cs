using Microsoft.EntityFrameworkCore;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class WeatherAppDbContext : DbContext
    {
        public WeatherAppDbContext(DbContextOptions<WeatherAppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<FavouriteLocation> FavouriteLocations { get; set; }
    }
}
