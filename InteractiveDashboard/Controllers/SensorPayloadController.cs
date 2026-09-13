using Microsoft.AspNetCore.Mvc;

namespace InteractiveDashboard.Controllers
{
    public class SensorPayloadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
