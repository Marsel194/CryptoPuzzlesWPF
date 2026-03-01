using CryptoPuzzles.Services.Api.Base;
using CryptoPuzzles.SharedDTO;
using System.Net.Http;
using System.Net.Http.Json;

namespace CryptoPuzzles.Services.ApiService
{
    public class AdminApiService(HttpClient httpClient) : BaseApiService(httpClient)
    {
        // READ: Получить всех админов
        public async Task<List<AAdmin>> GetAdminsAsync()
        {
            // Обрати внимание: путь "api/admins" должен совпадать с [Route] в контроллере
            return await SendAsync<List<AAdmin>>(() => _httpClient.GetAsync("api/admins"));
        }

        // DELETE: Удалить (мягкое удаление)
        public async Task<bool> DeleteAdminAsync(int id)
        {
            await SendAsync<string>(() => _httpClient.DeleteAsync($"api/admins/{id}"));
            return true;
        }

        // CREATE/UPDATE можно добавить по аналогии, когда допишем View
    }
}