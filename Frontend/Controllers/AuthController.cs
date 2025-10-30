using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
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

        public IActionResult Index()
        {
            return RedirectToAction("Login");
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
            string role = "";
            if (model.Email == "admin@ungana.com" && model.Password == "admin123")
            {
                role = "Admin";
            }
            else if (model.Email == "student@ungana.com" && model.Password == "student123")
            {
                role = "Student";
            }
            else if (model.Email == "instructor@ungana.com" && model.Password == "instructor123")
            {
                role = "Instructor";
            }
            else
            {
                ModelState.AddModelError("", "Invalid login credentials");
                return View(model);
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            HttpContext.Session.SetString("Token", $"fake-{role.ToLower()}-token");
            HttpContext.Session.SetString("Role", role);
            HttpContext.Session.SetString("UserEmail", model.Email);

            return RedirectToAction("Index", "Home");
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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}