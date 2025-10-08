using Newtonsoft.Json;
using System.Text;

namespace UnganaConnect.Frontend.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001/api";
        }

        public async Task<T?> GetAsync<T>(string endpoint, string? token = null)
        {
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            
            return default(T);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data, string? token = null)
        {
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            return await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data, string? token = null)
        {
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            return await _httpClient.PutAsync($"{_baseUrl}/{endpoint}", content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint, string? token = null)
        {
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
        }
    }
}