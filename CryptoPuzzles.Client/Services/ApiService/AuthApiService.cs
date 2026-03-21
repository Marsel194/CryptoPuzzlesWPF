using CryptoPuzzles.Client.Services.ApiService.Base;
using CryptoPuzzles.Shared;
using System.Net.Http;
using System.Net.Http.Json;

namespace CryptoPuzzles.Client.Services.ApiService
{
    public class AuthApiService : BaseApiService
    {
        public AuthApiService(HttpClient httpClient, UserSessionService userSessionService)
            : base(httpClient, userSessionService) { }

        public async Task<UALoginResponse> LoginAsync(string login, string password)
        {
            var request = new UALoginRequest(login, password);
            return await SendAsync<UALoginResponse>(() => _httpClient.PostAsJsonAsync("api/login", request));
        }

        public async Task<UARegisterResponse> RegisterAsync(UARegisterRequest request)
        {
            return await SendAsync<UARegisterResponse>(() => _httpClient.PostAsJsonAsync("api/login/register", request));
        }
    }
}