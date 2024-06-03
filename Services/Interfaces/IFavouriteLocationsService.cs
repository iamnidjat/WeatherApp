using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces
{
    public interface IFavouriteLocationsService
    {
        Task<IEnumerable<FavouriteLocation>> GetFLocationsAsync(int userId);
        Task AddFLocationAsync(FavouriteLocation favouriteLocation);
        Task DeleteFLocationAsync(int fLocationId);
        Task UpdateFLocationAsync(int fLocationId, FavouriteLocation favouriteLocation);
    }
}
