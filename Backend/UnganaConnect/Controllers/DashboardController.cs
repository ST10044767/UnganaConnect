using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Service.UI;
using UnganaConnect.Models.Training___Learning;
using UnganaConnect.Models.Event_Management;

namespace UnganaConnect.Controllers
{
    [Route("dashboard")]
    public class DashboardController : Controller
    {
        private readonly ApiService _apiService;
        private readonly AuthService _authService;

        public DashboardController(ApiService apiService, AuthService authService)
        {
            _apiService = apiService;
            _authService = authService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.IsAdmin = _authService.IsAdmin();
                ViewBag.UserEmail = _authService.GetUserEmail();

                var courses = await _apiService.GetAsync<List<UnganaConnect.Models.Training___Learning.Course>>("Course/get_course");
                var events = await _apiService.GetAsync<List<Event>>("Event");

                ViewBag.Courses = courses ?? new List<UnganaConnect.Models.Training___Learning.Course>();
                ViewBag.Events = events ?? new List<Event>();

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading dashboard: " + ex.Message;
                return View();
            }
        }
    }
}


