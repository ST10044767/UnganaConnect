using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Frontend.Models;

namespace UnganaConnect.Frontend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new DashboardViewModel
            {
                Stats = GetDashboardStats(),
                RecentCourses = GetRecentCourses(),
                UpcomingEvents = GetUpcomingEvents()
            };

            return View(viewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        private List<DashboardStat> GetDashboardStats()
        {
            return new List<DashboardStat>
            {
                new() { Title = "Courses Completed", Value = "12", Change = "+2 this month", Icon = "book-open", Color = "text-primary" },
                new() { Title = "Certificates Earned", Value = "8", Change = "+1 this week", Icon = "check-circle", Color = "text-success" },
                new() { Title = "Resources Downloaded", Value = "45", Change = "+8 this month", Icon = "download", Color = "text-info" },
                new() { Title = "Events Attended", Value = "6", Change = "+2 upcoming", Icon = "calendar", Color = "text-warning" }
            };
        }

        private List<RecentCourse> GetRecentCourses()
        {
            return new List<RecentCourse>
            {
                new() { Title = "Grant Writing Fundamentals", Progress = 85, TimeLeft = "2 hours left", Status = "In Progress" },
                new() { Title = "Financial Management for NGOs", Progress = 100, TimeLeft = "Completed", Status = "Completed" },
                new() { Title = "Digital Marketing Strategies", Progress = 45, TimeLeft = "4 hours left", Status = "In Progress" }
            };
        }

        private List<UpcomingEvent> GetUpcomingEvents()
        {
            return new List<UpcomingEvent>
            {
                new() { Title = "CSO Leadership Summit 2025", Date = "March 15, 2025", Type = "Conference", Location = "Virtual" },
                new() { Title = "Fundraising Workshop", Date = "March 22, 2025", Type = "Workshop", Location = "Hybrid" }
            };
        }
    }
}