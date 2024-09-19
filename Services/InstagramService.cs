using st10157545_giftgiversPOEs.Models;
using System.Text.Json;

namespace st10157545_giftgiversPOEs.Services
{
    public class InstagramService
    {
        private readonly HttpClient _httpClient;
        private const string AccessToken = "EAAMvPF3ZAcJwBO7qR9rWoyeknPylszrTPQSpUbhCgP5zJtHkxlh6fqxZAGQSIZBLnwbKbZCjb4WrobzelULiHvAirXCgzZAXkUN15WnRxJ69E9NkV6g3nzO4iwgKr0PZAXt6XJQHMkSetwB0KaScGlpmn7zMNo8nKQjiD6oTBwmhNpz6L8iVfR48ZCpc0g80c1tOwZDZD"; // Add your Instagram access token here
        private const int PageSize = 10;

        public InstagramService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<InstagramMediaResponse> GetDisasterMediaAsync(string hashtag = "disaster")
        {
            var requestUrl = $"https://graph.instagram.com/v12.0/{hashtag}/media?fields=id,caption,media_url,timestamp&access_token={AccessToken}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<InstagramMediaResponse>(content);
                }
                else
                {
                    throw new Exception($"Instagram API returned an error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Instagram media: {ex.Message}");
                return null;
            }
        }

        public async Task<InstagramMediaResponse> GetDisasterMediaAsync(string hashtag = "disaster", string nextPageUrl = null)
        {
            var requestUrl = nextPageUrl ?? $"https://graph.instagram.com/v12.0/{hashtag}/media?fields=id,caption,media_url,timestamp&limit={PageSize}&access_token={AccessToken}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<InstagramMediaResponse>(content);
                }
                else
                {
                    throw new Exception($"Instagram API returned an error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Instagram media: {ex.Message}");
                return null;
            }
        }

    }

}
