using CryptoPuzzles.Client.Services.ApiService.Base;
using CryptoPuzzles.Shared;
using System.Net.Http;

namespace CryptoPuzzles.Client.Services.ApiService
{
    public class UserApiService : BaseEntityApiService<AUser, AUserCreate, AUserUpdate>
    {
        public UserApiService(HttpClient httpClient, UserSessionService userSessionService)
            : base(httpClient, userSessionService, "api/users") { }
    }
}