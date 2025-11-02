using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UnganaConnect.Data;
using UnganaConnect.Frontend.Models;
using UnganaConnect.Models.Auth;
using UnganaConnect.Models.User;

namespace UnganaConnect.Controllers
{
    public class AuthController : Controller
    {
        private readonly UnganaConnectDbContext _context;

        public AuthController(UnganaConnectDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserEmail") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("Token", Guid.NewGuid().ToString());
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Name", ($"{user.FirstName} {user.LastName}").Trim());

            
            if (user.Role == "Member")
            {
                return RedirectToAction("Member", "Home");
            }
            else if (user.Role == "Admin")
            {
                return RedirectToAction("Admin", "Home");
            }
            else
            {
                
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserEmail") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

       
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("", "Email already exists.");
                return View(model);
            }
         

                var newUser = new User
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PasswordHash = HashPassword(model.Password),
                    Role = model.Role ?? "Member"
                };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }
    }
}
