using CryptoPuzzles.Shared;
using System.Net.Http;
using System.Text.Json;

namespace CryptoPuzzles.Services.ApiService.Base
{
    public abstract class BaseApiService(HttpClient httpClient)
    {
        protected readonly HttpClient _httpClient = httpClient;

        protected readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        protected async Task<T> SendAsync<T>(Func<Task<HttpResponseMessage>> action)
        {
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
            catch (HttpRequestException)
            {
                throw new Exception("Не удалось связаться с сервером. Проверьте подключение.");
            }
            catch (TaskCanceledException)
            {
                throw new Exception("Время ожидания ответа от сервера истекло.");
            }
        }
    }
}