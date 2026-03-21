using CryptoPuzzles.Client.Services.ApiService.Base;
using CryptoPuzzles.Shared;
using System.Net.Http;

namespace CryptoPuzzles.Client.Services.ApiService
{
    public class EncryptionMethodApiService : BaseEntityApiService<AEncryptionMethod, AEncryptionMethodCreate, AEncryptionMethodUpdate>
    {
        public EncryptionMethodApiService(HttpClient httpClient) : base(httpClient, "api/encryptionmethods") { }
    }
}