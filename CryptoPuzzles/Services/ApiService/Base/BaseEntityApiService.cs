using CryptoPuzzles.Services.Api.Base;
using System.Net.Http;
using System.Net.Http.Json;

namespace CryptoPuzzles.Services.ApiService.Base
{
    public abstract class BaseEntityApiService<T, TCreate, TUpdate> : BaseApiService, IEntityApiService<T, TCreate, TUpdate>
    {
        protected readonly string _endpoint;

        protected BaseEntityApiService(HttpClient httpClient, string endpoint) : base(httpClient)
        {
            _endpoint = endpoint;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await SendAsync<List<T>>(() => _httpClient.GetAsync(_endpoint));
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await SendAsync<T?>(() => _httpClient.GetAsync($"{_endpoint}/{id}"));
        }

        public async Task<T> CreateAsync(TCreate dto)
        {
            return await SendAsync<T>(() => _httpClient.PostAsJsonAsync(_endpoint, dto));
        }

        public async Task UpdateAsync(int id, TUpdate dto)
        {
            await SendAsync<object?>(() => _httpClient.PutAsJsonAsync($"{_endpoint}/{id}", dto));
        }
    }
}