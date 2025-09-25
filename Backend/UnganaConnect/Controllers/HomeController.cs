using Microsoft.AspNetCore.Mvc;

namespace UnganaConnect.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
