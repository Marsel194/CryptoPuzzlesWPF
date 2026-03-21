using CryptoPuzzles.Client.Services.ApiService.Base;
using CryptoPuzzles.Shared;
using System.Net.Http;

namespace CryptoPuzzles.Client.Services.ApiService
{
    public class TutorialApiService : BaseEntityApiService<ATutorial, ATutorialCreate, ATutorialUpdate>
    {
        public TutorialApiService(HttpClient httpClient, UserSessionService userSessionService)
            : base(httpClient, userSessionService, "api/tutorials") { }
    }
}