using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WeatherApp.Models.OpenWeatherMapModels;
using WeatherApp.Models;
using WeatherApp.Services.Interfaces;
using WeatherApp.ViewModels;
using WeatherApp.Config;
using WeatherApp.Services.Implementations;
using WeatherApp.Models.OpenMeteoModels;
using Newtonsoft.Json;

namespace WeatherApp.Controllers
{
    public class AirQualityController : Controller
    {
        private readonly IGetAirQualityDataRepository _getAirQualityDataRepository;
        private readonly IForecastRepository _forecastRepository;

        public AirQualityController(IGetAirQualityDataRepository getAirQualityDataRepository,
            IForecastRepository forecastRepository)
        {
            _getAirQualityDataRepository = getAirQualityDataRepository;
            _forecastRepository = forecastRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var SearchCityModel = new SearchCity();
            var AirQualityDataRequest = new AirQualityDataRequest(0, 0);
            List<SearchedCities> searchedCities = new List<SearchedCities>();

            var existingCitiesCookie = Request.Cookies["SearchedCitiesAQ"];
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
                AirQualityDataRequest = AirQualityDataRequest,
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
            var existingCitiesCookie = Request.Cookies["SearchedCitiesAQ"];
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
                Latitude = model.AirQualityDataRequest.Latitude,
                Longitude = model.AirQualityDataRequest.Longitude,
            };

            searchedCities.Add(newCity);

            // Serialize the updated list back to a cookie
            var newCitiesCookie = JsonConvert.SerializeObject(searchedCities);
            Response.Cookies.Append("SearchedCitiesAQ", newCitiesCookie, new CookieOptions
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
                    { "longitude", longitude },
                    { "pastDays", model.AirQualityDataRequest.PastDays }
                };

                if (model.AirQualityDataRequest.ForecastDays != 0)
                {
                    routeValues.Add("forecastDays", model.AirQualityDataRequest.ForecastDays);
                }
                else if (model.AirQualityDataRequest.StartDate != null && model.AirQualityDataRequest.EndDate != null)
                {
                    routeValues.Add("startDate", model.AirQualityDataRequest.StartDate);
                    routeValues.Add("endDate", model.AirQualityDataRequest.EndDate);
                }
                else if (model.AirQualityDataRequest.StartHour != null && model.AirQualityDataRequest.EndHour != null)
                {
                    routeValues.Add("startHour", model.AirQualityDataRequest.StartHour);
                    routeValues.Add("endHour", model.AirQualityDataRequest.EndHour);
                }

                return RedirectToAction("City", "AirQuality", routeValues);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult City(AirQualityDataRequest airQualityDataRequest)
        {
            AirQualityModel airQualityModel = null;
            var viewModel = new AirQualityDTO
            {
                Times = new List<string>(),
                Temperatures = new List<double?>()
            };
            NotFoundModel notFoundModel = new NotFoundModel();

            try
            {               
                airQualityModel = _getAirQualityDataRepository.GetAirQualityData(airQualityDataRequest);               

                if (airQualityModel != null)
                {
                    viewModel.Times.AddRange(airQualityModel.hourly.time);
                    viewModel.Temperatures.AddRange(airQualityModel.hourly.pm10);
                }
            }
            catch (CityNotFoundException ex)
            {
                viewModel = null;
                notFoundModel.ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                // _logger.LogError($"An error occurred: {ex.Message}");
                // Handle other exceptions if needed
            }

            var compositeAirQualitySearchModel = new CompositeAirQualitySearchModel
            {
                AirQualityDTO = viewModel,
                NotFoundModel = notFoundModel,
            };

            return View(compositeAirQualitySearchModel);
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
