using Newtonsoft.Json;
using System.Text;

namespace UnganaConnect.UI.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

            var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5217/api/";
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        private void AddAuthHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync(endpoint);
            return await HandleResponse<T>(response);
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            AddAuthHeader();
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            return await HandleResponse<T>(response);
        }

        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            AddAuthHeader();
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(endpoint, content);
            return await HandleResponse<T>(response);
        }

        public async Task<T> DeleteAsync<T>(string endpoint)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync(endpoint);
            return await HandleResponse<T>(response);
        }

        public async Task<byte[]> GetFileAsync(string endpoint)
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            throw new HttpRequestException($"Error downloading file: {response.StatusCode}");
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(string))
                    return (T)(object)content;

                return JsonConvert.DeserializeObject<T>(content) ?? default!;
            }

            throw new HttpRequestException($"API Error: {response.StatusCode} - {content}");
        }
    }
}
