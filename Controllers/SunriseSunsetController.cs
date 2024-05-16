using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models;
using WeatherApp.Models.OpenWeatherMapModels;
using WeatherApp.Models.SunriseSunsetModels;
using WeatherApp.Services.Interfaces;
using WeatherApp.ViewModels;

namespace WeatherApp.Controllers
{
    public class SunriseSunsetController : Controller
    {
        private readonly IGetAdditionalDataRepository _getAdditionalDataRepository;

        public SunriseSunsetController(IGetAdditionalDataRepository getAdditionalDataRepository)
        {
            _getAdditionalDataRepository = getAdditionalDataRepository;
        }

        [HttpPost]
        public IActionResult AdditionalData(string latlondata)
        {
            var data = latlondata.Split(',');
            //return RedirectToAction("AdditionalData", new { latitude = data[0], longitude = data[1] });
            return View(data[0], data[1]);
        }
        // /SunriseSunset/AdditionalData
        [HttpGet]
        public IActionResult AdditionalData(string latitude, string longitude)
        {
            Models.SunriseSunsetModels.Root sunriseSunsetModel = null;
            AdditionalData additionalData = new AdditionalData();
               
            sunriseSunsetModel = _getAdditionalDataRepository.GetAdditionalData(decimal.Parse(latitude), decimal.Parse(longitude));
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
    }
}
