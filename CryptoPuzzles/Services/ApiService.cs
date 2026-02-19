using Hairulin_02_01.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace Hairulin_02_01.Services
{
    internal class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://localhost:5206"; // твой URL API

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }


        public async Task<(bool canConnect, string message)> CheckDatabaseConnectionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/health/check-database");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<DatabaseCheckResult>();
                    return (result.canConnect, result.message);
                }
                else
                {
                    return (false, "Ошибка при проверке подключения к серверу");
                }
            }
            catch (HttpRequestException)
            {
                return (false, "Сервер недоступен.");
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
            catch (HttpRequestException)
            {
                throw new Exception("Сервер недоступен.");
            }
        }

        public async Task<User> LoginAsync(string login, string password)
        {
            try
            {
                string encodedLogin = Uri.EscapeDataString(login);
                string encodedPassword = Uri.EscapeDataString(password);

                var response = await _httpClient.GetAsync($"api/users/login?login={encodedLogin}&password={encodedPassword}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<User>();
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    throw new Exception(error?.message ?? "Ошибка входа");
                }
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
