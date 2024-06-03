using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WeatherApp.Config;
using WeatherApp.Models;
using WeatherApp.Services.Interfaces;
using WeatherApp.ViewModels;
using static WeatherApp.Services.Implementations.GetAdditionalDataRepository;

namespace WeatherApp.Controllers
{
    public class SunriseSunsetController : Controller
    {
        private readonly IGetAdditionalDataRepository _getAdditionalDataRepository;
        private readonly IForecastRepository _forecastRepository;

        public SunriseSunsetController(IGetAdditionalDataRepository getAdditionalDataRepository, IForecastRepository forecastRepository)
        {
            _getAdditionalDataRepository = getAdditionalDataRepository;
            _forecastRepository = forecastRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var SearchCityModel = new SearchCity();
            var SunriseSunsetModel = new SunriseSunsetModel(0, 0);
            List<SearchedCities> searchedCities = new List<SearchedCities>();

            var existingCitiesCookie = Request.Cookies["SearchedCitiesSS"];
            if (!string.IsNullOrEmpty(existingCitiesCookie))
            {
                try
                {
                    searchedCities = JsonConvert.DeserializeObject<List<SearchedCities>>(existingCitiesCookie);
                }
                catch (JsonException ex)
                {
                    // Log the error (ex) if necessary
                    // Initialize an empty list if deserialization fails
                    searchedCities = new List<SearchedCities>();
                }
            }

            var compositeModel = new CompositeViewModel
            {
                SearchCityModel = SearchCityModel,
                SunriseSunsetModel = SunriseSunsetModel,
                SearchedCities = searchedCities,
            };

            ViewData["LocationIQApiKey"] = Constants.LOCATION_IQ_KEY;

            return View(compositeModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CompositeViewModel model)
        {
            var latitude = 0f;
            var longitude = 0f;
            var existingCitiesCookie = Request.Cookies["SearchedCitiesSS"];
            var searchedCities = new List<SearchedCities>();

            if (!string.IsNullOrEmpty(existingCitiesCookie))
            {
                searchedCities = JsonConvert.DeserializeObject<List<SearchedCities>>(existingCitiesCookie);
            }

            // Add the new city to the list
            var newCity = new SearchedCities
            {
                Name = model.SearchCityModel.CityName,
                Date = DateTime.UtcNow,
                Latitude = model.SunriseSunsetModel.lat,
                Longitude = model.SunriseSunsetModel.lng,
            };

            searchedCities.Add(newCity);

            // Serialize the updated list back to a cookie
            var newCitiesCookie = JsonConvert.SerializeObject(searchedCities);
            Response.Cookies.Append("SearchedCitiesSS", newCitiesCookie, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1) // Cookie expires in 1 year
            });

            var weatherResponse = _forecastRepository.GetForecast(model.SearchCityModel.CityName);
            if (weatherResponse != null)
            {
                latitude = weatherResponse.Coord.Lat;
                longitude = weatherResponse.Coord.Lon;
            }

            // If the model is valid, consume the Weather API to bring the data of the city
            if (ModelState.IsValid)
            {
                var routeValues = new RouteValueDictionary
                {
                    { "latitude", latitude },
                    { "longitude", longitude }
                };

                if (model.SunriseSunsetModel.date != null)
                {
                    routeValues.Add("date", model.SunriseSunsetModel.date);
                }  

                return RedirectToAction("AdditionalData", "SunriseSunset", routeValues);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult AdditionalData(SunriseSunsetModel sunriseSunsetModelDTO)
        {
            Models.SunriseSunsetModels.Root sunriseSunsetModel = null;
            AdditionalData additionalData = new AdditionalData();
               
            sunriseSunsetModel = _getAdditionalDataRepository.GetAdditionalData(sunriseSunsetModelDTO);
            if (sunriseSunsetModel != null)
            {
                additionalData.Date = sunriseSunsetModel.results.date;
                additionalData.Sunrise = sunriseSunsetModel.results.sunrise;
                additionalData.Sunset = sunriseSunsetModel.results.sunset;
                additionalData.FirstLight = sunriseSunsetModel.results.first_light;
                additionalData.LastLight = sunriseSunsetModel.results.last_light;
                additionalData.Dawn = sunriseSunsetModel.results.dawn;
                additionalData.Dusk = sunriseSunsetModel.results.dusk;
                additionalData.SolarNoon = sunriseSunsetModel.results.solar_noon;
                additionalData.GoldenHour = sunriseSunsetModel.results.golden_hour;
                additionalData.DayLength = sunriseSunsetModel.results.day_length;
                additionalData.Timezone = sunriseSunsetModel.results.timezone;
            }
            else
            {
                return BadRequest("Invalid latitude or longitude format");
            }
               
            return View(additionalData);
        }

        [HttpPost]
        public IActionResult RemoveCity(string city)
        {
            // Retrieve existing cities from the cookie
            var existingCities = Request.Cookies["SearchedCitiesAQ"];
            if (existingCities != null)
            {
                // Split the existing cities into an array
                var cities = existingCities.Split(',').ToList();

                cities.Remove(city);

                // Join the remaining cities back into a string
                var updatedCities = string.Join(",", cities);

                // Update the cookie with the updated list of cities
                Response.Cookies.Append("SearchedCitiesAQ", updatedCities, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1) // Cookie expires in 1 year
                });
            }

            // Return a success response
            return Ok(new { success = true });
        }
    }
}
