using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Frontend.Models;

namespace UnganaConnect.Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly UnganaConnectDbContext _context;

        public HomeController(UnganaConnectDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
   return View();   
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public async Task<IActionResult> Member()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth", new { area = "" });

            var guidUserId = Guid.Parse(userId);

            // Fetch all enrollments including related Course
            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.UserId == guidUserId)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();

            // Calculate stats
            var completedCourses = enrollments.Count(e => e.Completed);
            var resources = await _context.Resources.ToListAsync();
            var resourceDownloads = resources.Sum(r => r.Downloads);

            var now = DateTime.UtcNow;

            var events = await _context.Events
                .Where(e => e.Date >= now)
                .OrderBy(e => e.Date)
                .Take(2)
                .ToListAsync();


            var userRegistrations = await _context.EventRegistrations
                .Where(r => r.UserEmail == userEmail)
                .CountAsync();

            // Build ViewModel
            var model = new DashboardViewModel
            {
                RecentCourses = enrollments
                    .Select((e, index) => new RecentCourse
                    {
                        Id = index + 1,
                        CourseId = e.CourseId,
                        Title = e.Course.Title,
                        Progress = (int)Math.Round(e.Progress),
                        TimeLeft = e.Completed ? "Completed" : "In progress",
                        Status = e.Completed ? "Completed" : "In Progress",
                        Course = e.Course
                    })
                    .Take(3)
                    .ToList(),

                EnrolledCourses = enrollments
                    .Select(e => new EnrolledCourse
                    {
                        Title = e.Course.Title,
                        Description = e.Course.Description,
                        ThumbnailUrl = e.Course.ThumbnailUrl
                    })
                    .Take(12)
                    .ToList(),

                UpcomingEvents = events
                    .Select(e => new UpcomingEvent
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Date = e.Date,
                        Type = e.Type,
                        Format = e.Format,
                        Location = e.Location
                    })
                    .ToList(),

                Stats = new DashboardStats
                {
                    CoursesCompleted = completedCourses,
                    CertificatesEarned = completedCourses,
                    ResourcesDownloaded = resourceDownloads,
                    EventsAttended = userRegistrations
                }
            };

            return View(model);
        }

        public async Task<IActionResult> Admin()
        {

            var now = DateTime.UtcNow;
            

           
            // Calculate admin stats
            var totalCourses = await _context.Courses.CountAsync();
            var activeUsers = await _context.Users.CountAsync();
            var totalResources = await _context.Resources.CountAsync();
            var upcomingEvents = await _context.Events
                .Where(e => e.Date >= now)
                .CountAsync();
            var events = await _context.Events
                            .Where(e => e.Date >= now)
                            .OrderBy(e => e.Date)
                            .Take(2)
                            .ToListAsync();

            var model = new DashboardViewModel
            {
                UpcomingEvents = events
                    .Select(e => new UpcomingEvent
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Date = e.Date,
                        Type = e.Type,
                        Format = e.Format,
                        Location = e.Location
                    })
                    .ToList(),

                Stats = new DashboardStats
                {
                    TotalCourses = totalCourses,
                    ActiveUsers = activeUsers,
                    TotalResources = totalResources,
                    UpcomingEventsCount = upcomingEvents
                }
            };

            return View(model);
        }
        
    }
}