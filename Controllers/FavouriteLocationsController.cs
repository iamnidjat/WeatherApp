using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Controllers
{
    [Route("FavouriteLocations")]
    public class FavouriteLocationsController : Controller
    {
        private readonly IFavouriteLocationsService _favouriteLocationsService;

        public FavouriteLocationsController(IFavouriteLocationsService favouriteLocationsService)
        {
            _favouriteLocationsService = favouriteLocationsService;
        }

        [HttpGet("Index/{userId:int}")]
        public async Task<IActionResult> Index(int userId)
        {
            var fLocations = await _favouriteLocationsService.GetFLocationsAsync(userId);
            return View(fLocations);
        }

        [HttpGet("AddFLocation/{userId:int}")]
        public IActionResult AddFLocation(int userId)
        {
            var favouriteLocation = new FavouriteLocation
            {
                UserId = userId
            };
            return View(favouriteLocation);
        }

        [HttpPost("AddFLocation")]
        public async Task<IActionResult> AddFLocation(FavouriteLocation fLocation)
        {
            if (ModelState.IsValid)
            {
                await _favouriteLocationsService.AddFLocationAsync(fLocation);
                return RedirectToAction("Index", new { userId = fLocation.UserId });
            }
            return View(fLocation);
        }

        [HttpDelete("DeleteFLocation")]
        public async Task<IActionResult> DeleteFLocation(int fLocationId)
        {
            await _favouriteLocationsService.DeleteFLocationAsync(fLocationId);
            return Ok(new { success = true });
        }

        [HttpPatch("UpdateFLocation")]
        public async Task<IActionResult> UpdateFLocation(int fLocationId, [FromBody] FavouriteLocation fLocation)
        {
            if (fLocation == null)
            {
                return BadRequest(new { success = false, message = "Invalid location data." });
            }

            await _favouriteLocationsService.UpdateFLocationAsync(fLocationId, fLocation);
            return Ok(new { success = true });
        }
    }
}
