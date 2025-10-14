using Microsoft.AspNetCore.Mvc;
using UnganaConnect.UI.Services;
using UnganaConnect.UI.Models;

namespace UnganaConnect.UI.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApiService _apiService;
        private readonly AuthService _authService;

        public CourseController(ApiService apiService, AuthService authService)
        {
            _apiService = apiService;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.IsAdmin = _authService.IsAdmin();
                ViewBag.UserEmail = _authService.GetUserEmail();
                
                var courses = await _apiService.GetAsync<List<Course>>("Course/get_course");
                return View(courses ?? new List<Course>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading courses: " + ex.Message;
                return View(new List<Course>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                ViewBag.IsAdmin = _authService.IsAdmin();
                ViewBag.UserEmail = _authService.GetUserEmail();
                
                var course = await _apiService.GetAsync<Course>($"Course/get_course/{id}");
                if (course == null)
                {
                    TempData["Error"] = "Course not found.";
                    return RedirectToAction("Index");
                }

                // Get modules for this course
                var modules = await _apiService.GetAsync<List<Module>>($"Module/course/{id}");
                course.Modules = modules ?? new List<Module>();

                return View(course);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading course details: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(int courseId)
        {
            if (!_authService.IsAuthenticated())
            {
                TempData["Error"] = "Please login to enroll in courses.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                await _apiService.PostAsync<object>($"Enrollment/{courseId}/enroll", new { });
                TempData["Success"] = "Successfully enrolled in course!";
                return RedirectToAction("Details", new { id = courseId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error enrolling in course: " + ex.Message;
                return RedirectToAction("Details", new { id = courseId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Unenroll(int courseId)
        {
            if (!_authService.IsAuthenticated())
            {
                TempData["Error"] = "Please login to manage enrollments.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                await _apiService.DeleteAsync<object>($"Enrollment/{courseId}/unenroll");
                TempData["Success"] = "Successfully unenrolled from course.";
                return RedirectToAction("Details", new { id = courseId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error unenrolling from course: " + ex.Message;
                return RedirectToAction("Details", new { id = courseId });
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!_authService.IsAdmin())
            {
                TempData["Error"] = "Access denied. Admin privileges required.";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Course course)
        {
            if (!_authService.IsAdmin())
            {
                TempData["Error"] = "Access denied. Admin privileges required.";
                return RedirectToAction("Index");
            }

            try
            {
                await _apiService.PostAsync<Course>("Course/create_course", course);
                TempData["Success"] = "Course created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error creating course: " + ex.Message;
                return View(course);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!_authService.IsAdmin())
            {
                TempData["Error"] = "Access denied. Admin privileges required.";
                return RedirectToAction("Index");
            }

            try
            {
                var course = await _apiService.GetAsync<Course>($"Course/get_course/{id}");
                if (course == null)
                {
                    TempData["Error"] = "Course not found.";
                    return RedirectToAction("Index");
                }
                return View(course);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading course: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Course course)
        {
            if (!_authService.IsAdmin())
            {
                TempData["Error"] = "Access denied. Admin privileges required.";
                return RedirectToAction("Index");
            }

            try
            {
                await _apiService.PutAsync<Course>($"Course/edit_course/{course.Id}", course);
                TempData["Success"] = "Course updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error updating course: " + ex.Message;
                return View(course);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_authService.IsAdmin())
            {
                TempData["Error"] = "Access denied. Admin privileges required.";
                return RedirectToAction("Index");
            }

            try
            {
                await _apiService.DeleteAsync<object>($"Course/delete_course/{id}");
                TempData["Success"] = "Course deleted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting course: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
