using CryptoPuzzles.Client.Services.ApiService.Base;
using CryptoPuzzles.Shared;
using System.Net.Http;

namespace CryptoPuzzles.Client.Services.ApiService
{
    public class HintApiService(HttpClient httpClient) : BaseEntityApiService<AHint, AHintCreate, AHintUpdate>(httpClient, "api/hints")
    {
    }
}