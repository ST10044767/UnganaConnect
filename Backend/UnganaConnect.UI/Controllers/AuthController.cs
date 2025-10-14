using Microsoft.AspNetCore.Mvc;
using UnganaConnect.UI.Services;

namespace UnganaConnect.UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Email and password are required.";
                return View();
            }

            var result = await _authService.LoginAsync(email, password);

            if (result.Success)
            {
                TempData["Success"] = "Login successful!";

                if (_authService.IsAuthenticated())
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }

            TempData["Error"] = result.Message;
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (_authService.IsAuthenticated())
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string password, string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                TempData["Error"] = "All fields are required.";
                return View();
            }

            var result = await _authService.RegisterAsync(email, password, firstName, lastName);

            if (result.Success)
            {
                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = result.Message;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            _authService.Logout();
            TempData["Success"] = "Logged out successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}
