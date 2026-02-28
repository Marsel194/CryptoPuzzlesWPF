using CryptoPuzzles.SharedDTO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace CryptoPuzzles.Services
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

        public async Task<AUser> RegisterAsync(UARegisterRequest request)  // Изменить сигнатуру
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/users/register", request);
                var content = await response.Content.ReadAsStringAsync();  // Сначала читаем как string

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<AUser>(content) ?? throw new Exception("Сервер вернул пустой ответ");
                }
                else
                {
                    try
                    {
                        var error = JsonSerializer.Deserialize<UAErrorResponse>(content);
                        throw new Exception(error?.Message ?? "Ошибка регистрации");
                    }
                    catch (JsonException)
                    {
                        throw new Exception($"Сервер вернул невалидный ответ: {content}");
                    }
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
                var response = await _httpClient.PostAsJsonAsync("api/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UALoginResponse>();
                    return result ?? throw new Exception("Сервер вернул пустой ответ");
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<UAErrorResponse>();
                    throw new Exception(error?.Message ?? "Ошибка входа");
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

        public async Task<List<AAdminDto>> GetAdmins()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/admins/get");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<AAdminDto>>()
                           ?? new List<AAdminDto>();
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
            public bool CanConnect { get; set; }
            public bool DatabaseCreated { get; set; }
            public string? Message { get; set; }
            public string? Error { get; set; }
        }
    }
}