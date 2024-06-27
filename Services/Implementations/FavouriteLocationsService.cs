using Microsoft.EntityFrameworkCore;
using WeatherApp.Models;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services.Implementations
{
    public class FavouriteLocationsService : IFavouriteLocationsService
    {
        private readonly WeatherAppDbContext _context;
        private readonly ILogger<FavouriteLocationsService> _logger;

        public FavouriteLocationsService(WeatherAppDbContext context, ILogger<FavouriteLocationsService> logger)
        {
            _context = context;
            _logger = logger;

        }

        public async Task AddFLocationAsync(FavouriteLocation favouriteLocation)
        {
            try
            {
                if (favouriteLocation != null)
                {
                    _context.FavouriteLocations.Add(favouriteLocation);

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex) when (ex is OperationCanceledException or DbUpdateException or DbUpdateConcurrencyException or ArgumentNullException)
            {
                _logger.LogError(ex, "An error occurred in the AddFLocationAsync method.");
            }
        }

        public async Task DeleteFLocationAsync(int fLocationId)
        {
            try
            {
                var film = await _context.FavouriteLocations.FirstOrDefaultAsync(f => f.Id == fLocationId);

                if (film != null)
                {
                    _context.FavouriteLocations.Remove(film);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    _logger.LogError("There is no location with this name!");
                }
            }
            catch (Exception ex) when (ex is OperationCanceledException or DbUpdateException or DbUpdateConcurrencyException or ArgumentNullException)
            {
                _logger.LogError(ex, "An error occurred in the DeleteFLocationAsync method.");
            }
        }

        public async Task<IEnumerable<FavouriteLocation>> GetFLocationsAsync(int userId)
        {
            try
            {
                return await _context.FavouriteLocations.Where(f => f.UserId == userId).ToListAsync();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "An error occurred in the GetFLocationsAsync method.");
                return Enumerable.Empty<FavouriteLocation>();
            }
        }

        public async Task UpdateFLocationAsync(int fLocationId, FavouriteLocation favouriteLocation)
        {
            if (favouriteLocation != null)
            {
                var location = await _context.FavouriteLocations.FirstOrDefaultAsync(f => f.Id == fLocationId);

                if (location != null)
                {
                    try
                    {
                        location.Name = favouriteLocation.Name;   

                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex) when (ex is OperationCanceledException or DbUpdateException or DbUpdateConcurrencyException or ArgumentNullException)
                    {
                        _logger.LogError(ex, "An error occurred in the UpdateFLocationAsync method.");
                    }
                }
                else
                {
                    _logger.LogError("There is no location with this name!");
                }
            }
            else
            {
                _logger.LogError("A location itself for updating is empty (or null)");
            }
        }
    }
}
