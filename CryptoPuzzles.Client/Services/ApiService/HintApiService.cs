using CryptoPuzzles.Client.Services.ApiService.Base;
using CryptoPuzzles.Shared;
using System.Net.Http;

namespace CryptoPuzzles.Client.Services.ApiService
{
    public class HintApiService : BaseEntityApiService<AHint, AHintCreate, AHintUpdate>
    {
        public HintApiService(HttpClient httpClient) : base(httpClient, "api/hints") { }
    }
}