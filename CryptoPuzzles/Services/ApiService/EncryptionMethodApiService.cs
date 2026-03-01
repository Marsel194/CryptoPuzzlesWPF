using CryptoPuzzles.SharedDTO;
using System.Net.Http;
using System.Security.Cryptography.Xml;

namespace CryptoPuzzles.Services.ApiService
{
    public class EncryptionMethodApiService : BaseEntityApiService<AEncryptionMethod, AEncryptionMethodCreate, AEncryptionMethodUpdate>
    {
        public EncryptionMethodApiService(HttpClient httpClient) : base(httpClient, "api/encryptionmethods") { }
    }
}