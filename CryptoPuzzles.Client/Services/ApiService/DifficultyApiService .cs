using CryptoPuzzles.Client.Services.ApiService.Base;
using CryptoPuzzles.Shared;
using System.Net.Http;

namespace CryptoPuzzles.Client.Services.ApiService
{
    public class DifficultyApiService : BaseEntityApiService<ADifficulty, ADifficultyCreate, ADifficultyUpdate>
    {
        public DifficultyApiService(HttpClient httpClient, UserSessionService userSessionService)
            : base(httpClient, userSessionService, "api/difficulties") { }
    }
}