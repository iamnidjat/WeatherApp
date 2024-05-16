using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models;
using WeatherApp.Models.OpenMeteoModels;
using WeatherApp.Models.OpenWeatherMapModels;
using WeatherApp.Services.Implementations;
using WeatherApp.Services.Interfaces;
using WeatherApp.ViewModels;

namespace WeatherApp.Controllers
{
   // [Route("Forecast")]
    public class ForecastController: Controller
    {
        private readonly IForecastRepository _forecastRepository;
        private readonly IPreciseForecastData _preciseForecastData;

        public ForecastController(IForecastRepository forecastAppRepo,
            IPreciseForecastData preciseForecastData)
        {
            _forecastRepository = forecastAppRepo;
            _preciseForecastData = preciseForecastData;
        }

        //[HttpGet("SearchCity")]
        [HttpGet]
        public IActionResult SearchCity()
        {
            var SearchCityModel = new SearchCity();
            var SystemModel = new WeatherApp.ViewModels.System();
          //  var ForecastDataRequest = new ForecastDataRequest();
            var cities = Request.Cookies["SearchedCities"]?.Split(',');

            var compositeModel = new CompositeViewModel
            {
                SearchCityModel = SearchCityModel,
                SystemModel = SystemModel,
               // ForecastDataRequest = ForecastDataRequest,
                SearchedCities = cities != null ? cities?.ToList() : new List<string>(),
            };

            return View(compositeModel);
        }

        //[HttpPost("SearchCity")]
        [HttpPost]
        public async Task<IActionResult> SearchCity(CompositeViewModel model)
        {
            var existingCities = Request.Cookies["SearchedCities"];

            // Append the new city to existing cities (if any)
            var newCities = existingCities != null ? $"{existingCities},{model.SearchCityModel.CityName}" : model.SearchCityModel.CityName;

            // Set the updated cities in the cookie
            Response.Cookies.Append("SearchedCities", newCities, new CookieOptions
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

        //  [HttpGet("City")]
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
                    viewModel.Weather = weatherResponse.Weather[0].Main;
                    viewModel.Wind = weatherResponse.Wind.Speed;
                    viewModel.WeatherIcon = $"http://openweathermap.org/img/w/{weatherResponse.Weather[0].Icon}.png";
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

        //[HttpPost]
        //public IActionResult AdditionalData(string latlondata)
        //{
        //    var data = latlondata.Split(',');
        //    return RedirectToAction("AdditionalData", "SunriseSunset", new { latitude = data[0], longitude = data[1] });
        //}

        //[HttpPost("RemoveCity")]
        [HttpPost]
        public IActionResult RemoveCity(string city)
        {
            // Retrieve existing cities from the cookie
            var existingCities = Request.Cookies["SearchedCities"];
            if (existingCities != null)
            {
                // Split the existing cities into an array
                var cities = existingCities.Split(',');

                // Find and remove the specified city from the array
                for (int i = 0; i < cities.Length; i++)
                {
                    if (cities[i] == city)
                    {
                        cities[i] = null; // Remove the city
                        break; // Stop searching
                    }
                }

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
            FullWeatherData viewModel = new FullWeatherData();

            if (preciseForecastData != null && forecastDataRequest.HourlyData != string.Empty)
            {
                viewModel.Times?.Add(preciseForecastData.hourly_units.temperature_2m);
            }
            else if (preciseForecastData != null && forecastDataRequest.DailyData != string.Empty)
            {
                viewModel.Times?.Add(preciseForecastData.daily_units.temperature_2m_max);
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SaveLocation(string latlondata)
        {
            // Process latlondata
            // You can save it to a database, perform calculations, etc.
            return Ok(); // Or return any other appropriate response
        }
    }
}
