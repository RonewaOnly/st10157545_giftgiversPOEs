using st10157545_giftgiversPOEs.Models;
using System.Text.Json;

namespace st10157545_giftgiversPOEs.Services
{
    public class FacebookService
    {
        private readonly HttpClient _httpClient;
        private const string AccessToken = "896361249206428|mecGtxX2RANQr6d1GwdG0PyJRLQ"; // Add your Facebook access token here
        private const int PageSize = 10;

        public FacebookService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<FacebookPostResponse> GetDisasterPostsAsync(string query = "disaster")
        {
            var requestUrl = $"https://graph.facebook.com/v12.0/search?q={query}&type=post&access_token={AccessToken}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<FacebookPostResponse>(content);
                }
                else
                {
                    throw new Exception($"Facebook API returned an error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Facebook posts: {ex.Message}");
                return null;
            }
        }

        public async Task<FacebookPostResponse> GetDisasterPostsAsync(string query = "disaster", string nextPageUrl = null)
        {
            var requestUrl = nextPageUrl ?? $"https://graph.facebook.com/v12.0/search?q={query}&type=post&limit={PageSize}&access_token={AccessToken}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<FacebookPostResponse>(content);
                }
                else
                {
                    throw new Exception($"Facebook API returned an error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Facebook posts: {ex.Message}");
                return null;
            }
        }

    }
}
