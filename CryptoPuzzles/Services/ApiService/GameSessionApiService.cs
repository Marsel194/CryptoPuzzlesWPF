using CryptoPuzzles.Shared;
using System.Net.Http;

namespace CryptoPuzzles.Services.ApiService
{
    public class GameSessionApiService : BaseEntityApiService<AGameSession, AGameSessionUpdate, AGameSessionUpdate>
    {
        public GameSessionApiService(HttpClient httpClient) : base(httpClient, "api/gamesessions") { }
    }
}