using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Frontend.Models;

namespace UnganaConnect.Frontend.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddVideo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddVideo(AddVideoViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Save video to database
                TempData["Success"] = "Video added successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult Users()
        {
            var users = GetUsers();
            return View(users);
        }

        public IActionResult Analytics()
        {
            var analytics = GetAnalytics();
            return View(analytics);
        }

        public IActionResult Courses()
        {
            var courses = GetCourses();
            return View(courses);
        }

        public IActionResult Resources()
        {
            var resources = GetResources();
            return View(resources);
        }

        [HttpPost]
        public IActionResult AddCourse(string title, string instructor)
        {
            // In a real app, this would save to database
            TempData["Success"] = $"Course '{title}' by {instructor} added successfully!";
            return RedirectToAction("Courses");
        }

        [HttpPost]
        public IActionResult AddResource(string title, string category, string type)
        {
            // In a real app, this would save to database
            TempData["Success"] = $"Resource '{title}' added successfully!";
            return RedirectToAction("Resources");
        }

        public IActionResult Events()
        {
            return View();
        }

        private List<AdminUser> GetUsers()
        {
            return new List<AdminUser>
            {
                new() { Id = 1, Name = "John Doe", Email = "john@example.com", Role = "Student", Status = "Active", LastLogin = "2 hours ago" },
                new() { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Role = "Instructor", Status = "Active", LastLogin = "1 day ago" },
                new() { Id = 3, Name = "Bob Wilson", Email = "bob@example.com", Role = "Student", Status = "Inactive", LastLogin = "1 week ago" }
            };
        }

        private AdminAnalytics GetAnalytics()
        {
            return new AdminAnalytics
            {
                TotalUsers = 1250,
                ActiveUsers = 890,
                TotalCourses = 12,
                CompletedCourses = 340,
                TotalResources = 45,
                ResourceDownloads = 2340
            };
        }

        private List<dynamic> GetCourses()
        {
            return new List<dynamic>
            {
                new { Id = 1, Title = "Web Development Fundamentals", Instructor = "David Kiptoo", Students = 145, Status = "Active" },
                new { Id = 2, Title = "Database Management Systems", Instructor = "Dr. Amara Okafor", Students = 98, Status = "Active" },
                new { Id = 3, Title = "Cloud Computing Essentials", Instructor = "Sarah Mensah", Students = 67, Status = "Active" }
            };
        }

        private List<dynamic> GetResources()
        {
            return new List<dynamic>
            {
                new { Id = 1, Title = "Grant Writing Templates", Category = "Grant Writing", Type = "Templates", Downloads = 245, Status = "Published" },
                new { Id = 2, Title = "Financial Management Toolkit", Category = "Finance", Type = "Toolkit", Downloads = 189, Status = "Published" }
            };
        }
    }
}