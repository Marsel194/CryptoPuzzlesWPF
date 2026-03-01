using CryptoPuzzles.SharedDTO;
using System.Net.Http;

namespace CryptoPuzzles.Services.ApiService
{
    public class UserApiService : BaseEntityApiService<AUser, AUserUpdate, AUserUpdate>
    {
        public UserApiService(HttpClient httpClient) : base(httpClient, "api/users") { }
    }
}