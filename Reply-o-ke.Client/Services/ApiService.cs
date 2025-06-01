using System.Net.Http.Json;
using Reply_o_ke.Client.Models;

namespace Reply_o_ke.Client.Services
{
    public partial class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SessionResponse?> CreateSessionAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/session/create", null);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<SessionResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating session: {ex.Message}");
                return null;
            }
        }

        public async Task<List<YouTubeVideo>> SearchVideosAsync(string query)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/youtube/search?query={Uri.EscapeDataString(query)}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<YouTubeVideo>>() ?? new List<YouTubeVideo>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching videos: {ex.Message}");
                return new List<YouTubeVideo>();
            }
        }

        public async Task<List<QueueItem>> GetQueueAsync(string sessionId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/session/{sessionId}/queue");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<QueueItem>>() ?? new List<QueueItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting queue: {ex.Message}");
                return new List<QueueItem>();
            }
        }

        public async Task RemoveQueueItemAsync(string sessionId, int itemId)
        {
            var response = await _httpClient.DeleteAsync($"api/session/{sessionId}/queue/{itemId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<SessionResponse?> GetSessionAsync(string sessionId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/session/{sessionId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<SessionResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting session: {ex.Message}");
                return null;
            }
        }
    }

    public class SessionResponse
    {
        public string SessionId { get; set; } = string.Empty;
        public string JoinUrl { get; set; } = string.Empty;
        public string QRCode { get; set; } = string.Empty;
    }
}
