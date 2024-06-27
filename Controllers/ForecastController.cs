using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Security.Claims;
using WeatherApp.Config;
using WeatherApp.Models;
using WeatherApp.Models.OpenMeteoModels;
using WeatherApp.Models.OpenWeatherMapModels;
using WeatherApp.Services.Implementations;
using WeatherApp.Services.Interfaces;
using WeatherApp.ViewModels;

namespace WeatherApp.Controllers
{
    public class ForecastController : Controller
    {
        private readonly IForecastRepository _forecastRepository;
        private readonly IPreciseForecastData _preciseForecastData;
        private readonly IGetAirQualityDataRepository _getAirQualityDataRepository;

        public ForecastController(IForecastRepository forecastAppRepo, 
            IPreciseForecastData preciseForecastData, IGetAirQualityDataRepository getAirQualityDataRepository,
            IStringLocalizer<SearchCity> localizer)
        {
            _forecastRepository = forecastAppRepo;
            _preciseForecastData = preciseForecastData;
            _getAirQualityDataRepository = getAirQualityDataRepository;
        }

        [HttpGet]
        [Route("{controller=Forecast}/{action=SearchCity}")]
        public async Task<IActionResult> SearchCity()
        {
            if (Request.Cookies["IsAuthenticated"] == "true")
            {
                // Perform automatic login
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, User.Identity.Name)
                };

                ClaimsIdentity id = new(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
            }

            var SearchCityModel = new SearchCity();
            var SystemModel = new WeatherApp.ViewModels.System();
            var ForecastDataRequest = new ForecastDataRequest(0, 0);
            var AirQualityDataRequest = new AirQualityDataRequest(0, 0);
            var searchedCities = new List<SearchedCities>();

            SystemModel.Units = "metric";

            var existingCitiesCookie = Request.Cookies["SearchedCities"];
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
                SystemModel = SystemModel,
                ForecastDataRequest = ForecastDataRequest,
                AirQualityDataRequest = AirQualityDataRequest,
                SearchedCities = searchedCities,
            };

            ViewData["LocationIQApiKey"] = Constants.LOCATION_IQ_KEY;

            return View(compositeModel);
        }

        [HttpPost]
        public async Task<IActionResult> SearchCity(CompositeViewModel model)
        {
            var existingCitiesCookie = Request.Cookies["SearchedCities"];
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
                System = model.SystemModel.Units,
            };

            searchedCities.Add(newCity);

            // Serialize the updated list back to a cookie
            var newCitiesCookie = JsonConvert.SerializeObject(searchedCities);
            Response.Cookies.Append("SearchedCities", newCitiesCookie, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1) // Cookie expires in 1 year
            });

            // If the model is valid, consume the Weather API to bring the data of the city
            if (ModelState.IsValid)
            {
                return RedirectToAction("City", "Forecast", new { city = model.SearchCityModel.CityName, system = model.SystemModel.Units });
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult City(string city, string system)
        {
            WeatherResponse weatherResponse = null;
            City viewModel = new City();
            NotFoundModel notFoundModel = new NotFoundModel();
           
            try
            {
                weatherResponse = _forecastRepository.GetForecast(city, system);
                if (weatherResponse != null)
                {
                    viewModel.Name = weatherResponse.Name;
                    viewModel.Humidity = weatherResponse.Main.Humidity;
                    viewModel.Pressure = weatherResponse.Main.Pressure;
                    viewModel.Temp = weatherResponse.Main.Temp;
                    viewModel.MinTemp = weatherResponse.Main.Temp_Min;
                    viewModel.MaxTemp = weatherResponse.Main.Temp_Max;
                    viewModel.FeelsLike = weatherResponse.Main.Feels_Like;
                    viewModel.Weather = weatherResponse.Weather[0].Main;
                    viewModel.WeatherDescription = weatherResponse.Weather[0].Description;
                    viewModel.Wind = weatherResponse.Wind.Speed;
                    viewModel.WeatherIcon = $"http://openweathermap.org/img/w/{weatherResponse.Weather[0].Icon}.png";
                    viewModel.Visibility = weatherResponse.Visibility;
                    viewModel.Latitude = weatherResponse.Coord.Lat;
                    viewModel.Longitude = weatherResponse.Coord.Lon;
                }
            }
            catch (CityNotFoundException ex)
            {
                viewModel = null;
                notFoundModel.ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
              //  _logger.LogError($"An error occurred: {ex.Message}");
                // Handle other exceptions if needed
            }

            var compositeSearchModel = new CompositeSearchModel
            {
                City = viewModel,
                NotFoundModel = notFoundModel,
            };

            return View(compositeSearchModel);
        }

        [HttpPost]
        public IActionResult RemoveCity(string city)
        {
            // Retrieve existing cities from the cookie
            var existingCities = Request.Cookies["SearchedCities"];
            if (existingCities != null)
            {
                // Split the existing cities into an array
                var cities = existingCities.Split(',').ToList();

                // Remove the specified city from the list
                cities.Remove(city);

                // Join the remaining cities back into a string
                var updatedCities = string.Join(",", cities);

                // Update the cookie with the updated list of cities
                Response.Cookies.Append("SearchedCities", updatedCities, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1) // Cookie expires in 1 year
                });
            }

            // Return a success response
            return Ok(new { success = true });
        }


        [HttpGet]
        public IActionResult FullWeatherData(ForecastDataRequest forecastDataRequest)
        {
            PreciseForecastData preciseForecastData = _preciseForecastData.GetForecastData(forecastDataRequest);
            var viewModel = new FullWeatherData
            {
                Times = new List<string>(),
                Temperatures = new List<double>()
            };

            if (preciseForecastData != null)
            {
                if (!string.IsNullOrEmpty(forecastDataRequest.HourlyData))
                {
                    viewModel.Times.AddRange(preciseForecastData.hourly.time);
                    viewModel.Temperatures.AddRange(preciseForecastData.hourly.temperature_2m);
                }
                else if (!string.IsNullOrEmpty(forecastDataRequest.DailyData))
                {
                    viewModel.Times.AddRange(preciseForecastData.daily.time);
                    viewModel.Temperatures.AddRange(preciseForecastData.daily.temperature_2m_max);
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AirQuality(AirQualityDataRequest airQualityDataRequest)
        {
            AirQualityModel airQualityModel = _getAirQualityDataRepository.GetAirQualityData(airQualityDataRequest);
            var viewModel = new AirQualityDTO
            {
                Times = new List<string>(),
                Temperatures = new List<double?>()
            };

            if (airQualityModel != null)
            {
                viewModel.Times.AddRange(airQualityModel.hourly.time);
                viewModel.Temperatures.AddRange(airQualityModel.hourly.pm10);              
            }

            return View(viewModel);
        }
    }
}
