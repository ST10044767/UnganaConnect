namespace UnganaConnect.Service.UI
{
    public class AuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken")
                        ?? _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            return !string.IsNullOrEmpty(token);
        }

        public bool IsAdmin()
        {
            var role = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
            return role == "Admin";
        }

        public string GetUserEmail()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("UserEmail") ?? string.Empty;
        }
    }
}


