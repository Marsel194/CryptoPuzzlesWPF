namespace CryptoPuzzles.Services.ApiService
{
    public interface IEntityApiService<T, TCreate, TUpdate>
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> CreateAsync(TCreate dto);
        Task UpdateAsync(int id, TUpdate dto);
    }
}