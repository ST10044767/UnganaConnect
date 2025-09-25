using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UnganaConnect.Data;
using UnganaConnect.Models.Users;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace UnganaConnect.Controllers.Auth
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UnganaConnectDbcontext _context;
        private readonly IConfiguration _config;

        public AuthController(UnganaConnectDbcontext context, IConfiguration config)
        {
            // Context 
            _context = context;
            // Configrution 
            _config = config;
        }

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
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            // Generate JWT
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                message = "Login successful",
                token,
                role = user.Role.ToString()
            });
        }

        
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUsers()
        {
            return Ok(_context.Users.ToList());
        }

        // MVC Actions

        [HttpGet("~/Auth/Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("~/Auth/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View(request);
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View(request);
            }

            // Sign in with cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("~/Auth/Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("~/Auth/Register")]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User newUser)
        {
            if (_context.Users.Any(u => u.Email == newUser.Email))
            {
                ModelState.AddModelError("", "Email already registered.");
                return View(newUser);
            }

            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.UpdatedAt = DateTime.UtcNow;
            newUser.Role = newUser.Email == "admin@admin.com" ? "Admin" : "Member";

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpGet("~/Auth/Users")]
        [Authorize(Roles = "Admin")]
        public IActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        [HttpGet("~/Auth/Profile")]
        [Authorize]
        public IActionResult Profile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);
            var user = _context.Users.Find(userId);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost("~/Auth/Profile")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(User updatedUser)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);
            var user = _context.Users.Find(userId);
            if (user == null) return NotFound();

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            // etc.
            _context.SaveChanges();
            return RedirectToAction("Profile");
        }

        [HttpPost("~/Auth/Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("~/Auth/AdminDashboard")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
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


}
