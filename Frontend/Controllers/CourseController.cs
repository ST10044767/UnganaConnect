using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Frontend.Models;
using UnganaConnect.Frontend.Services;

namespace UnganaConnect.Frontend.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApiService _apiService;

        public CourseController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string search = "", string category = "all")
        {
            var courses = GetSampleCourses();
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
                new() { Id = 6, Title = "Data Analysis for Social Impact", Description = "Use data to measure and improve the effectiveness of your programs.", Instructor = "Alex Kim", Duration = "9 hours", Level = "Advanced", Rating = 4.5, Enrolled = 87, Status = "available", Category = "Analytics", Thumbnail = "https://images.unsplash.com/photo-1745847768367-893e989d3a98?w=400" }
            };
        }

        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = HttpContext.Session.GetString("Token");
            var response = await _apiService.PostAsync("course/create_course", model, token);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Course created successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to create course");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return RedirectToAction("Index");

            var course = await _apiService.GetAsync<CourseViewModel>($"course/{id}");
            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CourseViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = HttpContext.Session.GetString("Token");
            var response = await _apiService.PutAsync($"course/edit_course/{id}", model, token);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Course updated successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to update course");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("Token");
            var response = await _apiService.DeleteAsync($"course/delete_course/{id}", token);
            
            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Course deleted successfully!";
            else
                TempData["Error"] = "Failed to delete course";

            return RedirectToAction("Index");
        }
    }
}