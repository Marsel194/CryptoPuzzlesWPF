using CryptoPuzzles.SharedDTO;
using System.Net.Http;

namespace CryptoPuzzles.Services.ApiService
{
    public class HintApiService : BaseEntityApiService<AHint, AHintCreate, AHintUpdate>
    {
        public HintApiService(HttpClient httpClient) : base(httpClient, "api/hints") { }
    }
}