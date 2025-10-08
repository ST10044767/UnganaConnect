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

            var response = await _apiService.PostAsync("auth/login", model);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(content);
                
                HttpContext.Session.SetString("Token", loginResponse.Token);
                HttpContext.Session.SetString("Role", loginResponse.Role);
                
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

            var userModel = new
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = model.Password,
                Role = model.Role
            };

            var response = await _apiService.PostAsync("auth/register", userModel);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Registration failed");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}