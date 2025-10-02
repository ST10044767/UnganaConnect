using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;

namespace UnganaConnect.Controllers
{
    public class ImpactMvcController : Controller
    {
        private readonly UnganaConnectDbcontext _context;

        public ImpactMvcController(UnganaConnectDbcontext context)
        {
            _context = context;
        }

        // GET: Impact/Overview
        public async Task<IActionResult> Overview()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalCourses = await _context.Courses.CountAsync();
            var totalEnrollments = await _context.Enrollments.CountAsync();
            var totalResources = await _context.Resources.CountAsync();
            var totalConsultancyRequests = await _context.ConsultancyRequests.CountAsync();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalCourses = totalCourses;
            ViewBag.TotalEnrollments = totalEnrollments;
            ViewBag.TotalResources = totalResources;
            ViewBag.TotalConsultancyRequests = totalConsultancyRequests;

            return View();
        }

        // GET: Impact/Interactive
        public async Task<IActionResult> Interactive()
        {
            // Placeholder for interactive dashboard
            return View();
        }

        // GET: Impact/Report
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Report()
        {
            // Generate a simple report
            var reportData = new
            {
                Users = await _context.Users.CountAsync(),
                Courses = await _context.Courses.CountAsync(),
                Enrollments = await _context.Enrollments.CountAsync(),
                Resources = await _context.Resources.CountAsync(),
                ConsultancyRequests = await _context.ConsultancyRequests.CountAsync()
            };

            return View(reportData);
        }
    }
}
