using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        [HttpGet]
        public IActionResult NotFound()
        {
            return View();
        }
    }
}
