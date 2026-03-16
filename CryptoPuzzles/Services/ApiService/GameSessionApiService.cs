using CryptoPuzzles.Services.ApiService.Base;
using CryptoPuzzles.Shared;
using System.Net.Http;

namespace CryptoPuzzles.Services.ApiService
{
    public class GameSessionApiService : BaseEntityApiService<AGameSession, AGameSessionCreate, AGameSessionUpdate>
    {
        public GameSessionApiService(HttpClient httpClient) : base(httpClient, "api/gamesessions") { }

        public async Task<List<AGameSession>> GetAllAsync(int? userId = null, string? sessionType = null, bool? isCompleted = null)
        {
            var query = _endpoint;
            var parameters = new List<string>();
            if (userId.HasValue) parameters.Add($"userId={userId.Value}");
            if (!string.IsNullOrEmpty(sessionType)) parameters.Add($"sessionType={sessionType}");
            if (isCompleted.HasValue) parameters.Add($"isCompleted={isCompleted.Value}");
            if (parameters.Any()) query += "?" + string.Join("&", parameters);
            return await SendAsync<List<AGameSession>>(() => _httpClient.GetAsync(query));
        }
    }
}