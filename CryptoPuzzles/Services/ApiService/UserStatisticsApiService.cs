using CryptoPuzzles.Shared;
using System.Net.Http;

namespace CryptoPuzzles.Services.ApiService
{
    public class UserStatisticsApiService : BaseEntityApiService<AUserStatistic, object, object>
    {
        public UserStatisticsApiService(HttpClient httpClient) : base(httpClient, "api/userstatistics") { }

        public async Task<AUserStatistic?> GetByUserIdAsync(int userId)
        {
            return await SendAsync<AUserStatistic?>(() => _httpClient.GetAsync($"{_endpoint}/user/{userId}"));
        }

        public async Task<List<AUserStatistic>> GetLeaderboardAsync(int top = 10, string criteria = "totalScore")
        {
            return await SendAsync<List<AUserStatistic>>(() =>
                _httpClient.GetAsync($"{_endpoint}/leaderboard?top={top}&criteria={criteria}"));
        }

        public async Task<object> GetUserProgressOverTimeAsync(int userId)
        {
            return await SendAsync<object>(() => _httpClient.GetAsync($"{_endpoint}/user/{userId}/progress"));
        }

        public async Task RefreshStatisticsAsync(int userId)
        {
            var response = await _httpClient.PostAsync($"{_endpoint}/refresh/{userId}", null);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Ошибка при обновлении статистики: {response.StatusCode} - {errorContent}");
            }
        }
    }
}