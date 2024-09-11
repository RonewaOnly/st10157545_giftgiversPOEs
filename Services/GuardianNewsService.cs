using st10157545_giftgiversPOEs.Models;
using System.Text.Json;

namespace st10157545_giftgiversPOEs.Services
{
    public class GuardianNewsService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "ef1a9e21-0b94-4b9d-9873-028444e78ed1"; // mY Guardian API key
        private const string BaseUrl = "https://content.guardianapis.com/";

        public GuardianNewsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GuardianNewsResponse> GetLatestNewsAsync(string section = "world")
        {
            var requestUrl = $"{BaseUrl}{section}?api-key={ApiKey}&show-fields=all";
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GuardianNewsResponse>(content);
            }
            return null;
        }

    }
}
