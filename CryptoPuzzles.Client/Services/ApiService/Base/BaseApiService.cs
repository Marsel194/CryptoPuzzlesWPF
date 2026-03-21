// BaseApiService.cs (добавляем возможность распознавать 401 по коду статуса)
using CryptoPuzzles.Client.Services;
using CryptoPuzzles.Shared;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CryptoPuzzles.Client.Services.ApiService.Base
{
    public abstract class BaseApiService
    {
        protected readonly HttpClient _httpClient;
        protected readonly UserSessionService _userSessionService;
        protected readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        protected BaseApiService(HttpClient httpClient, UserSessionService userSessionService)
        {
            _httpClient = httpClient;
            _userSessionService = userSessionService;
        }

        protected void SetAuthHeader(HttpRequestMessage request)
        {
            var token = _userSessionService.Token;
            if (request.Headers.Authorization != null)
                return;

            if (_httpClient.DefaultRequestHeaders.Authorization != null)
                return;

            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected async Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            SetAuthHeader(request);

            try
            {
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return default!;

                    if (typeof(T) == typeof(string))
                        return (T)(object)content;

                    return JsonSerializer.Deserialize<T>(content, _jsonOptions)
                           ?? throw new Exception("Сервер вернул пустой ответ");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new Exception("Unauthorized (401)");

                if (string.IsNullOrWhiteSpace(content))
                    throw new Exception($"Ошибка сервера: {response.StatusCode}");

                try
                {
                    var error = JsonSerializer.Deserialize<UAErrorResponse>(content, _jsonOptions);
                    throw new Exception(error?.Message ?? $"Ошибка сервера: {response.StatusCode}");
                }
                catch (JsonException)
                {
                    throw new Exception($"Непредвиденная ошибка: {content}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Не удалось связаться с сервером. Проверьте подключение.");
            }
            catch (TaskCanceledException ex)
            {
                throw new Exception("Время ожидания ответа от сервера истекло.");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected async Task<T> SendAsync<T>(Func<Task<HttpResponseMessage>> action)
        {
            SetAuthHeaderToDefault();

            try
            {
                var response = await action().ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return default!;

                    if (typeof(T) == typeof(string))
                        return (T)(object)content;

                    return JsonSerializer.Deserialize<T>(content, _jsonOptions)
                           ?? throw new Exception("Сервер вернул пустой ответ");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new Exception("Unauthorized (401)");

                if (string.IsNullOrWhiteSpace(content))
                    throw new Exception($"Ошибка сервера: {response.StatusCode}");

                try
                {
                    var error = JsonSerializer.Deserialize<UAErrorResponse>(content, _jsonOptions);
                    throw new Exception(error?.Message ?? $"Ошибка сервера: {response.StatusCode}");
                }
                catch (JsonException)
                {
                    throw new Exception($"Непредвиденная ошибка: {content}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Не удалось связаться с сервером. Проверьте подключение.");
            }
            catch (TaskCanceledException ex)
            {
                throw new Exception("Время ожидания ответа от сервера истекло.");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void SetAuthHeaderToDefault()
        {
            var token = _userSessionService.Token;
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            else
                _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}