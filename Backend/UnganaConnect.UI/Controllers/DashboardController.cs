using Microsoft.AspNetCore.Mvc;
using UnganaConnect.UI.Services;
using UnganaConnect.UI.Models;

namespace UnganaConnect.UI.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApiService _apiService;
        private readonly AuthService _authService;

        public DashboardController(ApiService apiService, AuthService authService)
        {
            _apiService = apiService;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
          

            try
            {
                var viewModel = new DashboardViewModel();

                // Set ViewBag properties for layout
                ViewBag.IsAdmin = _authService.IsAdmin();
                ViewBag.UserEmail = _authService.GetUserEmail();

                // Get courses
                var courses = await _apiService.GetAsync<List<Course>>("Course/get_course");
                viewModel.Courses = courses ?? new List<Course>();

                // Get events
                var events = await _apiService.GetAsync<List<Event>>("Event");
                viewModel.Events = events ?? new List<Event>();

                // Get user enrollments if not admin
                if (!_authService.IsAdmin())
                {
                    var enrollments = await _apiService.GetAsync<List<Enrollment>>("Enrollment/my-enrollments");
                    viewModel.Enrollments = enrollments ?? new List<Enrollment>();
                }

                // Get admin dashboard data if admin
                if (_authService.IsAdmin())
                {
                    var adminData = await _apiService.GetAsync<AdminDashboardData>("Admin/dashboard");
                    viewModel.AdminData = adminData;
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading dashboard: " + ex.Message;
                return View(new DashboardViewModel());
            }
        }
    }

    public class DashboardViewModel
    {
        public List<Course> Courses { get; set; } = new();
        public List<Event> Events { get; set; } = new();
        public List<Enrollment> Enrollments { get; set; } = new();
        public AdminDashboardData? AdminData { get; set; }
    }

    public class AdminDashboardData
    {
        public StatisticsData Statistics { get; set; } = new();
        public RecentActivityData RecentActivity { get; set; } = new();
    }

    public class StatisticsData
    {
        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public int TotalEvents { get; set; }
        public int TotalThreads { get; set; }
        public int TotalResources { get; set; }
    }

    public class RecentActivityData
    {
        public List<Enrollment> RecentEnrollments { get; set; } = new();
        public List<Event> RecentEvents { get; set; } = new();
    }
}
