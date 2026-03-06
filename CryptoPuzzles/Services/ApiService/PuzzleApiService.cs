using CryptoPuzzles.Shared;
using System.Net.Http;

namespace CryptoPuzzles.Services.ApiService
{
    public class PuzzleApiService : BaseEntityApiService<APuzzle, APuzzleCreate, APuzzleUpdate>
    {
        public PuzzleApiService(HttpClient httpClient) : base(httpClient, "api/puzzles") { }
    }
}