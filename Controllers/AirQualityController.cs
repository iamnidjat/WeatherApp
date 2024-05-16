using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Controllers
{
    public class AirQualityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
