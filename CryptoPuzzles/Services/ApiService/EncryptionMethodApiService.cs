using CryptoPuzzles.Shared;
using System.Net.Http;

namespace CryptoPuzzles.Services.ApiService
{
    public class EncryptionMethodApiService : BaseEntityApiService<AEncryptionMethod, AEncryptionMethodCreate, AEncryptionMethodUpdate>
    {
        public EncryptionMethodApiService(HttpClient httpClient) : base(httpClient, "api/encryptionmethods") { }
    }
}