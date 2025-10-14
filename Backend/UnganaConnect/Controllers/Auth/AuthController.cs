using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UnganaConnect.Data;
using UnganaConnect.Models.Users;
using BCrypt.Net;

namespace UnganaConnect.Controllers.Auth
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UnganaConnectDbcontext _context;
        private readonly IConfiguration _config;

        public AuthController(UnganaConnectDbcontext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // =============================
        // ======== API ROUTES =========
        // =============================

        [HttpPost("register")]
        public IActionResult ApiRegister([FromBody] User newUser)
        {
            if (_context.Users.Any(u => u.Email == newUser.Email))
                return BadRequest("Email already registered.");

            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.UpdatedAt = DateTime.UtcNow;

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new { message = "Registration successful", userId = newUser.Id });
        }

        [HttpPost("login")]
        public IActionResult ApiLogin([FromBody] LoginRequest request)
        {
            var result = ValidateUserCredentials(request.Email, request.Password);
            if (result == null)
                return Unauthorized("Invalid credentials.");

            var token = GenerateJwtToken(result);

            return Ok(new
            {
                message = "Login successful",
                token,
                role = result.Role.ToString()
            });
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUsers()
        {
            return Ok(_context.Users.ToList());
        }

        // =============================
        // ======== MVC ROUTES =========
        // =============================

        [HttpGet("/login")]
        public IActionResult LoginView()
        {
            return View("Login");
        }

        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        public IActionResult LoginViewPost(string email, string password)
        {
            var user = ValidateUserCredentials(email, password);
            if (user == null)
            {
                TempData["Error"] = "Invalid credentials.";
                return View("Login");
            }

            // Generate and store JWT token (optional for session use)
            var token = GenerateJwtToken(user);
            HttpContext.Session.SetString("JwtToken", token);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet("/register")]
        public IActionResult RegisterView()
        {
            return View("Register");
        }

        [HttpPost("/register")]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterViewPost(string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
            {
                TempData["Error"] = "Email already registered.";
                return View("Register");
            }

            var newUser = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Role = "User"
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            TempData["Success"] = "Registration successful. You can now log in.";
            return RedirectToAction("LoginView");
        }

        // =============================
        // ======== HELPERS ============
        // =============================

        private User? ValidateUserCredentials(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return user;
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var keyString = jwtSettings["Key"];

            if (string.IsNullOrEmpty(keyString))
                throw new Exception("JWT Key is missing. Check appsettings.json or environment variables.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }



    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
