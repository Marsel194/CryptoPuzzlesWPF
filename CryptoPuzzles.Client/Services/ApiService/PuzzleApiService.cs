using CryptoPuzzles.Client.Services.ApiService.Base;
using CryptoPuzzles.Shared;
using System.Net.Http;

namespace CryptoPuzzles.Client.Services.ApiService
{
    public class PuzzleApiService : BaseEntityApiService<APuzzle, APuzzleCreate, APuzzleUpdate>
    {
        public PuzzleApiService(HttpClient httpClient, UserSessionService userSessionService)
            : base(httpClient, userSessionService, "api/puzzles") { }
    }
}