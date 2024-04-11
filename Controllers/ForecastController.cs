using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Reflection;
using WeatherApp.Models;
using WeatherApp.Models.OpenWeatherMapModels;
using WeatherApp.Services.Interfaces;
using WeatherApp.ViewModels;

namespace WeatherApp.Controllers
{
    public class ForecastController: Controller
    {
        private readonly IForecastRepository _forecastRepository;

        // Dependency Injection
        public ForecastController(IForecastRepository forecastAppRepo)
        {
            _forecastRepository = forecastAppRepo;
        }
        // GET: ForecastApp/SearchCity
        public IActionResult SearchCity()
        {
            var SearchCityModel = new SearchCity();
            var SystemModel = new WeatherApp.ViewModels.System();
            var cities = Request.Cookies["SearchedCities"]?.Split(',');

            var compositeModel = new CompositeViewModel
            {
                SearchCityModel = SearchCityModel,
                SystemModel = SystemModel,
                SearchedCities = cities != null ? cities?.ToList() : new List<string>(),
            };
            return View(compositeModel);
        }

        // POST: ForecastApp/SearchCity
        [HttpPost]
        public async Task<IActionResult> SearchCity(CompositeViewModel model)
        {
            var existingCities = Request.Cookies["SearchedCities"];

            // Append the new city to existing cities (if any)
            var newCities = existingCities != null ? $"{existingCities},{model.SearchCityModel.CityName},{DateTime.Now.ToShortDateString}" : $"{model.SearchCityModel.CityName},{ DateTime.Now.ToShortDateString}";

            // Set the updated cities in the cookie
            Response.Cookies.Append("SearchedCities", newCities, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1) // Cookie expires in 1 year
            });
            // If the model is valid, consume the Weather API to bring the data of the city
            if (ModelState.IsValid)
            {
              //  string cityName = await GetCityFromIP();
                return RedirectToAction("City", "Forecast", new { city = model.SearchCityModel.CityName, system = model.SystemModel.Units });
            }
            return View(model);
        }

        // GET: ForecastApp/City
        public IActionResult City(string city, string system)
        {
            WeatherResponse weatherResponse = _forecastRepository.GetForecast(city, system);
            City viewModel = new City();

            if (weatherResponse != null)
            {
                viewModel.Name = weatherResponse.Name;
                viewModel.Humidity = weatherResponse.Main.Humidity;
                viewModel.Pressure = weatherResponse.Main.Pressure;
                viewModel.Temp = weatherResponse.Main.Temp;
                viewModel.Weather = weatherResponse.Weather[0].Main;
                viewModel.Wind = weatherResponse.Wind.Speed;
            }

            return View(viewModel);
        }

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

        //private async Task<string> GetCityFromIP()
        //{
        //    string userIp = GetIPAddress();
        //    string apiUrl = $"http://ip-api.com/json/{userIp}";

        //    using (HttpClient client = new HttpClient())
        //    {
        //        HttpResponseMessage response = await client.GetAsync(apiUrl);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            string jsonResult = await response.Content.ReadAsStringAsync();
        //            return jsonResult;
        //        }
        //    }

        //    return null;
        //}

        //public string GetIPAddress()
        //{
        //    //IPHostEntry Host = default(IPHostEntry);
        //    //string Hostname = null;
        //    //Hostname = System.Environment.MachineName;
        //    //Host = Dns.GetHostEntry(Hostname);
        //    //foreach (IPAddress IP in Host.AddressList)
        //    //{
        //    //    if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        //    //    {
        //    //        string MyIPAddress = Convert.ToString(IP);
        //    //        return MyIPAddress;
        //    //    }
        //    //}
        //    //return null;


        //    //IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());

        //    //// Find the first IPv4 address
        //    //foreach (IPAddress ip in hostAddresses)
        //    //{
        //    //    if (ip.AddressFamily == AddressFamily.InterNetwork)
        //    //    {
        //    //        return ip.ToString();
        //    //    }
        //    //}

        //    //return null;





        //}
    }
}
