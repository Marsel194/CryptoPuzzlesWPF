using System.Net.Http;
using System.Net.Http.Json;

namespace CryptoPuzzles.Client.Services.ApiService.Base
{
    public abstract class BaseEntityApiService<T, TCreate, TUpdate> : BaseApiService, IEntityApiService<T, TCreate, TUpdate>
    {
        protected readonly string _endpoint;

        protected BaseEntityApiService(HttpClient httpClient, UserSessionService userSessionService, string endpoint)
            : base(httpClient, userSessionService)
        {
            _endpoint = endpoint;
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await SendAsync<List<T>>(() => _httpClient.GetAsync(_endpoint));
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await SendAsync<T?>(() => _httpClient.GetAsync($"{_endpoint}/{id}"));
        }

        public virtual async Task<T> CreateAsync(TCreate dto)
        {
            return await SendAsync<T>(() => _httpClient.PostAsJsonAsync(_endpoint, dto));
        }

        public virtual async Task UpdateAsync(int id, TUpdate dto)
        {
            await SendAsync<object?>(() => _httpClient.PutAsJsonAsync($"{_endpoint}/{id}", dto));
        }
    }
}