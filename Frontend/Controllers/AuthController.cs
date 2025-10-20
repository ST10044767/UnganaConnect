using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Frontend.Models;
using UnganaConnect.Frontend.Services;
using Newtonsoft.Json;

namespace UnganaConnect.Frontend.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;

        public AuthController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Simulate successful login without backend
            if (model.Email == "admin@ungana.com" && model.Password == "admin123")
            {
                HttpContext.Session.SetString("Token", "fake-admin-token");
                HttpContext.Session.SetString("Role", "Admin");
                HttpContext.Session.SetString("UserEmail", model.Email);
                return RedirectToAction("Index", "Home");
            }
            else if (model.Email == "student@ungana.com" && model.Password == "student123")
            {
                HttpContext.Session.SetString("Token", "fake-student-token");
                HttpContext.Session.SetString("Role", "Student");
                HttpContext.Session.SetString("UserEmail", model.Email);
                return RedirectToAction("Index", "Home");
            }
            else if (model.Email == "instructor@ungana.com" && model.Password == "instructor123")
            {
                HttpContext.Session.SetString("Token", "fake-instructor-token");
                HttpContext.Session.SetString("Role", "Instructor");
                HttpContext.Session.SetString("UserEmail", model.Email);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login credentials");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Simulate successful registration without backend
            TempData["Success"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}