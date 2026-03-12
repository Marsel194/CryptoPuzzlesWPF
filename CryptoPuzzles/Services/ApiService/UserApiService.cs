using CryptoPuzzles.Shared;
using System.Net.Http;

namespace CryptoPuzzles.Services.ApiService
{
    public class UserApiService : BaseEntityApiService<AUser, AUserCreate, AUserUpdate>
    {
        public UserApiService(HttpClient httpClient) : base(httpClient, "api/users") { }
    }
}