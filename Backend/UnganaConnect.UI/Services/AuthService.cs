using Newtonsoft.Json;
using System.Text;

namespace UnganaConnect.UI.Services
{
    public class AuthService
    {
        private readonly ApiService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(ApiService apiService, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            try
            {
                var loginData = new { email, password };
                var result = await _apiService.PostAsync<LoginResult>("Auth/login", loginData);

                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    _httpContextAccessor.HttpContext?.Session.SetString("JWTToken", result.Token);
                    _httpContextAccessor.HttpContext?.Session.SetString("UserRole", result.Role ?? "Member");
                    _httpContextAccessor.HttpContext?.Session.SetString("UserEmail", email);
                }

                return result ?? new LoginResult { Success = false, Message = "Login failed" };
            }
            catch (Exception ex)
            {
                return new LoginResult { Success = false, Message = ex.Message };
            }
        }

        public async Task<RegisterResult> RegisterAsync(string email, string password, string firstName, string lastName)
        {
            try
            {
                var registerData = new { email, passwordHash = password, firstName, lastName };
                var result = await _apiService.PostAsync<RegisterResult>("Auth/register", registerData);
                return result ?? new RegisterResult { Success = false, Message = "Registration failed" };
            }
            catch (Exception ex)
            {
                return new RegisterResult { Success = false, Message = ex.Message };
            }
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
        }

        public bool IsAuthenticated()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            return !string.IsNullOrEmpty(token);
        }

        public bool IsAdmin()
        {
            var role = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
            return role == "Admin";
        }

        public string GetUserEmail()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("UserEmail") ?? "";
        }

        public string GetUserRole()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("UserRole") ?? "Member";
        }
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
    }

    public class RegisterResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int UserId { get; set; }
    }
}
