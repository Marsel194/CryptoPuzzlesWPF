using CryptoPuzzles.SharedDTO;
using System.Net.Http;

namespace CryptoPuzzles.Services.ApiService
{
    public class AdminApiService : BaseEntityApiService<AAdmin, AAdminCreate, AAdminUpdate>
    {
        public AdminApiService(HttpClient httpClient) : base(httpClient, "api/admins")
        {
        }
    }
}