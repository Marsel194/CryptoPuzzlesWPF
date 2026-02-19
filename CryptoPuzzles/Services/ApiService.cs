using Hairulin_02_01.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace Hairulin_02_01.Services
{
    internal class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:44352";

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

        public async Task<User> RegisterAsync(User user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/users/register", user);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<User>();
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    throw new Exception(error?.message ?? "Ошибка регистрации");
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

        public async Task<LoginResponse> LoginAsync(string login, string password)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Login = login,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync("api/users/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LoginResponse>();
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    throw new Exception(error?.message ?? "Ошибка входа");
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
