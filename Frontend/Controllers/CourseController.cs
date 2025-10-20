using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Frontend.Models;
using UnganaConnect.Frontend.Services;

namespace UnganaConnect.Frontend.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApiService _apiService;
        private static List<CourseViewModel> _courses = new();

        public CourseController(ApiService apiService)
        {
            _apiService = apiService;
            if (_courses.Count == 0)
            {
                _courses = GetSampleCourses();
            }
        }

        public async Task<IActionResult> Index(string search = "", string category = "all")
        {
            var role = HttpContext.Session.GetString("Role");
            var courses = role == "Admin" ? _courses : _courses.Where(c => c.Status != "draft").ToList();
            var categories = new List<string> { "all", "Fundraising", "Finance", "Marketing", "Management", "Community", "Analytics" };

            var filteredCourses = courses.Where(c =>
                (string.IsNullOrEmpty(search) || c.Title.Contains(search, StringComparison.OrdinalIgnoreCase) || c.Description.Contains(search, StringComparison.OrdinalIgnoreCase)) &&
                (category == "all" || c.Category == category)
            ).ToList();

            var model = new CourseCatalogViewModel
            {
                Courses = filteredCourses,
                Categories = categories,
                SearchTerm = search,
                SelectedCategory = category
            };

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var course = GetSampleCourses().FirstOrDefault(c => c.Id == id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(int id)
        {
            var token = HttpContext.Session.GetString("Token");
            var response = await _apiService.PostAsync($"course/enroll/{id}", new { }, token);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Successfully enrolled in course!";
            }
            else
            {
                TempData["Error"] = "Failed to enroll in course.";
            }

            return RedirectToAction("Details", new { id });
        }

        private List<CourseViewModel> GetSampleCourses()
        {
            return new List<CourseViewModel>
            {
                new() { Id = 1, Title = "Grant Writing Fundamentals", Description = "Learn the essentials of writing compelling grant proposals that secure funding for your organization.", Instructor = "Dr. Sarah Williams", Duration = "6 hours", Level = "Beginner", Rating = 4.8, Enrolled = 245, Progress = 85, Status = "enrolled", Category = "Fundraising", Thumbnail = "https://images.unsplash.com/photo-1726831662518-c48d983f9b86?w=400" },
                new() { Id = 2, Title = "Financial Management for NGOs", Description = "Master financial planning, budgeting, and reporting for non-profit organizations.", Instructor = "Michael Chen", Duration = "8 hours", Level = "Intermediate", Rating = 4.9, Enrolled = 189, Progress = 100, Status = "completed", Category = "Finance", Thumbnail = "https://images.unsplash.com/photo-1675242314995-034d11bac319?w=400" },
                new() { Id = 3, Title = "Digital Marketing for Social Impact", Description = "Leverage digital platforms to amplify your mission and reach more supporters.", Instructor = "Emma Rodriguez", Duration = "5 hours", Level = "Beginner", Rating = 4.7, Enrolled = 312, Progress = 45, Status = "enrolled", Category = "Marketing", Thumbnail = "https://images.unsplash.com/photo-1675119715594-30fde4bd3dbc?w=400" },
                new() { Id = 4, Title = "Project Management Essentials", Description = "Learn project management methodologies tailored for civil society organizations.", Instructor = "James Thompson", Duration = "7 hours", Level = "Intermediate", Rating = 4.6, Enrolled = 156, Status = "available", Category = "Management", Thumbnail = "https://images.unsplash.com/photo-1646579886741-12b59840c63f?w=400" },
                new() { Id = 5, Title = "Community Engagement Strategies", Description = "Build stronger relationships with your community through effective engagement techniques.", Instructor = "Dr. Priya Patel", Duration = "4 hours", Level = "Beginner", Rating = 4.8, Enrolled = 203, Status = "available", Category = "Community", Thumbnail = "https://images.unsplash.com/photo-1555069855-e580a9adbf43?w=400" },
                new() { Id = 6, Title = "Data Analysis for Social Impact", Description = "Use data to measure and improve the effectiveness of your programs.", Instructor = "Alex Kim", Duration = "9 hours", Level = "Advanced", Rating = 4.5, Enrolled = 87, Status = "available", Category = "Analytics", Thumbnail = "https://images.unsplash.com/photo-1745847768367-893e989d3a98?w=400" },
                new() { Id = 7, Title = "Monitoring and Evaluation Basics", Description = "Learn fundamental concepts of M&E for development projects.", Instructor = "Dr. Lisa Johnson", Duration = "6 hours", Level = "Beginner", Rating = 4.6, Enrolled = 134, Status = "available", Category = "Analytics", Thumbnail = "https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=400" }
            };
        }

        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
                return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Simulate course creation without backend
            model.Id = _courses.Max(c => c.Id) + 1;
            model.CreatedAt = DateTime.Now;
            model.Status = "draft"; // Admin can publish later
            _courses.Add(model);

            TempData["Success"] = "Course created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
                return RedirectToAction("Index");

            var course = _courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CourseViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var course = _courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
                return NotFound();

            // Update course properties
            course.Title = model.Title;
            course.Description = model.Description;
            course.Category = model.Category;
            course.Instructor = model.Instructor;
            course.Duration = model.Duration;
            course.Level = model.Level;
            course.Thumbnail = model.Thumbnail;

            TempData["Success"] = "Course updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
                return RedirectToAction("Index");

            var course = _courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
                return NotFound();

            _courses.Remove(course);
            TempData["Success"] = "Course deleted successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Publish(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return RedirectToAction("Index");

            var course = _courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
                return NotFound();

            course.Status = "available";
            TempData["Success"] = "Course published successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult MyLearning()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Auth");
            }

            var enrolledCourses = _courses.Where(c => c.Status == "enrolled" || c.Status == "completed").ToList();
            var model = new CourseCatalogViewModel
            {
                Courses = enrolledCourses,
                Categories = new List<string> { "all", "Fundraising", "Finance", "Marketing", "Management", "Community", "Analytics" },
                SearchTerm = "",
                SelectedCategory = "all"
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Certificate(int id)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Auth");
            }

            var course = _courses.FirstOrDefault(c => c.Id == id && c.Status == "completed");
            if (course == null)
            {
                TempData["Error"] = "Certificate not available for this course.";
                return RedirectToAction("MyLearning");
            }

            var certificateModel = new CertificateViewModel
            {
                CourseTitle = course.Title,
                Instructor = course.Instructor,
                CompletionDate = DateTime.Now.ToString("MMMM dd, yyyy"),
                UserName = HttpContext.Session.GetString("UserName") ?? "Learner",
                CertificateId = $"CERT-{id}-{DateTime.Now.Year}"
            };

            return View(certificateModel);
        }

        [HttpGet]
        public IActionResult DownloadCertificate(int id)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Auth");
            }

            var course = _courses.FirstOrDefault(c => c.Id == id && c.Status == "completed");
            if (course == null)
            {
                TempData["Error"] = "Certificate not available for this course.";
                return RedirectToAction("MyLearning");
            }

            // Generate PDF content (simplified - in real app, use a PDF library)
            var certificateContent = $@"
CERTIFICATE OF COMPLETION

This certifies that

{HttpContext.Session.GetString("UserName") ?? "Learner"}

has successfully completed the course

{course.Title}

Instructor: {course.Instructor}
Completion Date: {DateTime.Now.ToString("MMMM dd, yyyy")}
Certificate ID: CERT-{id}-{DateTime.Now.Year}

Ungana Connect Learning Platform
";

            var fileName = $"Certificate_{course.Title.Replace(" ", "_")}_{DateTime.Now.Year}.txt";
            var contentType = "text/plain";

            return File(System.Text.Encoding.UTF8.GetBytes(certificateContent), contentType, fileName);
        }
    }
}