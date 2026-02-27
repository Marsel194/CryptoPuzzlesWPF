using CryptoPuzzles.SharedDTO;
using System.Net.Http;
using System.Net.Http.Json;

namespace Hairulin_02_01.Services
{
    internal class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://localhost:5206";
        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromSeconds(20)
            };
        }

        public async Task<bool> IsServerAliveAsync()
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                var response = await _httpClient.GetAsync("api/health/ping", cts.Token);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UUser> RegisterAsync(UUser user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/users/register", user);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UUser>();
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<UAErrorResponse>();
                    throw new Exception(error?.Message ?? "Ошибка регистрации");
                }
            }
            catch (TaskCanceledException)
            {
                throw new Exception("Сервер не ответил вовремя. Проверьте интернет или попробуйте позже.");
            }
            catch (HttpRequestException)
            {
                throw new Exception("Сервер недоступен.");
            }
        }

        public async Task<UALoginResponse> LoginAsync(string login, string password)
        {
            try
            {
                var loginRequest = new UALoginRequest(login, password);

                var response = await _httpClient.PostAsJsonAsync("api/users/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UALoginResponse>();
                }
                else
                {
                    response = await _httpClient.PostAsJsonAsync("api/admins/login", loginRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        
                         return await response.Content.ReadFromJsonAsync<UALoginResponse>();
                    }
                    else
                    {
                        var error = await response.Content.ReadFromJsonAsync<UAErrorResponse>();
                        throw new Exception(error?.Message ?? "Ошибка входа");
                    }
                }
            }
            catch (TaskCanceledException)
            {
                throw new Exception("Сервер не ответил вовремя. Проверьте интернет.");
            }
            catch (HttpRequestException)
            {
                throw new Exception("Сервер недоступен.");
            }
        }

        public async Task<AAdminDto> GetAdmins()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/admins/get");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AAdminDto>();
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<UAErrorResponse>();
                    throw new Exception(error?.Message ?? "Ошибка получения данных");
                }
            }
            catch (TaskCanceledException)
            {
                throw new Exception("Сервер не ответил вовремя. Проверьте интернет.");
            }
            catch (HttpRequestException)
            {
                throw new Exception("Сервер недоступен.");
            }
        }
        public class DatabaseCheckResult
        {
            public bool canConnect { get; set; }
            public bool databaseCreated { get; set; }
            public string message { get; set; }
            public string error { get; set; }
        }
    }
}
