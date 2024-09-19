using Microsoft.Extensions.Caching.Memory;
using st10157545_giftgiversPOEs.Models;
using System.Text.Json;

namespace st10157545_giftgiversPOEs.Services
{
    public class TwitterService
    {
        private readonly HttpClient _httpClient;
        private const string BearerToken = "AAAAAAAAAAAAAAAAAAAAADRdvwEAAAAA1JJekhgO0TjzXy6IN5H1BLqLTuE%3DEEnI5fN28RmpqbUzLhWN90jt6qiDVXj5omhBuifsJuiNn7DKmk"; // Add your Twitter Bearer Token here

        private readonly IMemoryCache _cache;
        private const string CacheKey = "TwitterDisasterNews";
        private const int PageSize = 10; // Define the number of results per page


        public TwitterService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<TwitterSearchResponse> SearchDisasterNewsAsync(string query = "disaster OR earthquake OR flood")
        {
            if (!_cache.TryGetValue(CacheKey, out TwitterSearchResponse cachedResponse))
            {
                var requestUrl = $"https://api.twitter.com/2/tweets/search/recent?query={query}&tweet.fields=created_at,author_id&expansions=author_id";
                _httpClient.DefaultRequestHeaders.Authorization = null; // Clear any existing Authorization header

                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {BearerToken.Trim()}");

                try
                {
                    var response = await _httpClient.GetAsync(requestUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        cachedResponse = JsonSerializer.Deserialize<TwitterSearchResponse>(content);

                        // Store in cache for 15 minutes
                        _cache.Set(CacheKey, cachedResponse, TimeSpan.FromMinutes(15));
                    }
                    else
                    {
                        throw new Exception($"Twitter API returned an error: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching Twitter news: {ex.Message}");
                    return null;
                }
            }
            return cachedResponse;
        }

        public async Task<TwitterSearchResponse> SearchDisasterNewsAsync(string query = "disaster OR earthquake OR flood", string nextToken = null)
        {
            var requestUrl = $"https://api.twitter.com/2/tweets/search/recent?query={query}&max_results={PageSize}&tweet.fields=created_at,author_id&expansions=author_id";

            if (!string.IsNullOrEmpty(nextToken))
            {
                requestUrl += $"&next_token={nextToken}";
            }
            _httpClient.DefaultRequestHeaders.Authorization = null; // Clear any existing Authorization header


            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {BearerToken.Trim()}");

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TwitterSearchResponse>(content);
                }
                else
                {
                    throw new Exception($"Twitter API returned an error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Twitter news: {ex.Message}");
                return null;
            }
        }

        public async Task<TwitterSearchResponse> SearchDisasterNewsAsync(string query = "disaster", string country = null, string nextToken = null)
        {
            var fullQuery = query;

            if (!string.IsNullOrEmpty(country))
            {
                fullQuery += $" AND place_country:{country}";
            }

            var requestUrl = $"https://api.twitter.com/2/tweets/search/recent?query={fullQuery}&max_results={PageSize}&tweet.fields=created_at,author_id&expansions=author_id";

            if (!string.IsNullOrEmpty(nextToken))
            {
                requestUrl += $"&next_token={nextToken}";
            }
            _httpClient.DefaultRequestHeaders.Authorization = null; // Clear any existing Authorization header

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {BearerToken.Trim()}");

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TwitterSearchResponse>(content);
                }
                else
                {
                    throw new Exception($"Twitter API returned an error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Twitter news: {ex.Message}");
                return null;
            }
        }
    }
}

