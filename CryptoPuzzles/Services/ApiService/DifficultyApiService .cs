using CryptoPuzzles.Services.ApiService.Base;
using CryptoPuzzles.Shared;
using System.Net.Http;

namespace CryptoPuzzles.Services.ApiService
{
    public class DifficultyApiService : BaseEntityApiService<ADifficulty, ADifficultyCreate, ADifficultyUpdate>
    {
        public DifficultyApiService(HttpClient httpClient) : base(httpClient, "api/difficulties") { }
    }
}