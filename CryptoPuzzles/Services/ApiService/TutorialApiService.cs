using CryptoPuzzles.Services.ApiService.Base;
using CryptoPuzzles.Shared;
using System.Net.Http;

namespace CryptoPuzzles.Services.ApiService
{
    public class TutorialApiService : BaseEntityApiService<ATutorial, ATutorialCreate, ATutorialUpdate>
    {
        public TutorialApiService(HttpClient httpClient) : base(httpClient, "api/tutorials") { }
    }
}