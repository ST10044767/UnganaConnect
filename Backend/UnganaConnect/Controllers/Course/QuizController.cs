using Microsoft.AspNetCore.Mvc;

namespace UnganaConnect.Controllers.Course
{
    public class QuizController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
