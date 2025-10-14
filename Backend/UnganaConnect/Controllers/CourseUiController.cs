using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Service.UI;
using UnganaConnect.Models.Training___Learning;

namespace UnganaConnect.Web.Controllers
{
    [Route("course")]
    public class CourseController : Controller
    {
        private readonly ApiService _apiService;
        private readonly AuthService _authService;

        public CourseController(ApiService apiService, AuthService authService)
        {
            _apiService = apiService;
            _authService = authService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewBag.IsAdmin = _authService.IsAdmin();
            ViewBag.UserEmail = _authService.GetUserEmail();
            var courses = await _apiService.GetAsync<List<Course>>("Course/get_course");
            return View(courses ?? new List<Course>());
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            ViewBag.IsAdmin = _authService.IsAdmin();
            ViewBag.UserEmail = _authService.GetUserEmail();
            var course = await _apiService.GetAsync<Course>($"Course/get_course/{id}");
            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Index");
            }
            var modules = await _apiService.GetAsync<List<Module>>($"Module/course/{id}");
            course.Modules = modules ?? new List<Module>();
            return View(course);
        }

        [HttpPost("enroll")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            if (!_authService.IsAuthenticated())
            {
                TempData["Error"] = "Please login to enroll in courses.";
                return RedirectToAction("LoginView", "Auth");
            }
            await _apiService.PostAsync<object>($"Enrollment/{courseId}/enroll", new { });
            TempData["Success"] = "Successfully enrolled in course!";
            return RedirectToAction("Details", new { id = courseId });
        }

        [HttpPost("unenroll")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unenroll(int courseId)
        {
            if (!_authService.IsAuthenticated())
            {
                TempData["Error"] = "Please login to manage enrollments.";
                return RedirectToAction("LoginView", "Auth");
            }
            await _apiService.DeleteAsync<object>($"Enrollment/{courseId}/unenroll");
            TempData["Success"] = "Successfully unenrolled from course.";
            return RedirectToAction("Details", new { id = courseId });
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            if (!_authService.IsAdmin())
            {
                TempData["Error"] = "Access denied. Admin privileges required.";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (!_authService.IsAdmin())
            {
                TempData["Error"] = "Access denied. Admin privileges required.";
                return RedirectToAction("Index");
            }
            await _apiService.PostAsync<Course>("Course/create_course", course);
            TempData["Success"] = "Course created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (!_authService.IsAdmin())
            {
                TempData["Error"] = "Access denied. Admin privileges required.";
                return RedirectToAction("Index");
            }
            var course = await _apiService.GetAsync<Course>($"Course/get_course/{id}");
            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Index");
            }
            return View(course);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
        {
            if (!_authService.IsAdmin())
            {
                TempData["Error"] = "Access denied. Admin privileges required.";
                return RedirectToAction("Index");
            }
            await _apiService.PutAsync<Course>($"Course/edit_course/{id}", course);
            TempData["Success"] = "Course updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_authService.IsAdmin())
            {
                TempData["Error"] = "Access denied. Admin privileges required.";
                return RedirectToAction("Index");
            }
            await _apiService.DeleteAsync<object>($"Course/delete_course/{id}");
            TempData["Success"] = "Course deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}


