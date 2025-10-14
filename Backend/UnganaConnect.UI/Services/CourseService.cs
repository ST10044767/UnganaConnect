using Newtonsoft.Json;
using System.Text;

namespace UnganaConnect.UI.Services
{
    public class CourseService
    {
        private readonly ApiService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CourseService(ApiService apiService, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _httpContextAccessor = httpContextAccessor;
        }

    }
}