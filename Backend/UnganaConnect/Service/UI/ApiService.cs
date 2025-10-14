using Newtonsoft.Json;
using System.Text;

namespace UnganaConnect.Service.UI
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient("UiApiClient");
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken")
                        ?? _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        private string BuildApiUrl(string endpoint)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return $"/api/{endpoint}";
            var scheme = httpContext.Request.Scheme;
            var host = httpContext.Request.Host.Value;
            return $"{scheme}://{host}/api/{endpoint}";
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var client = CreateClient();
            var response = await client.GetAsync(BuildApiUrl(endpoint));
            return await HandleResponse<T>(response);
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            var client = CreateClient();
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(BuildApiUrl(endpoint), content);
            return await HandleResponse<T>(response);
        }

        public async Task<T?> PutAsync<T>(string endpoint, object data)
        {
            var client = CreateClient();
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(BuildApiUrl(endpoint), content);
            return await HandleResponse<T>(response);
        }

        public async Task<T?> DeleteAsync<T>(string endpoint)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync(BuildApiUrl(endpoint));
            return await HandleResponse<T>(response);
        }

        private static async Task<T?> HandleResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)content;
                }
                return JsonConvert.DeserializeObject<T>(content);
            }
            throw new HttpRequestException($"API Error: {response.StatusCode} - {content}");
        }
    }
}


