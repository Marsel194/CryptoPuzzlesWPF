using CryptoPuzzles.Server.Models;

namespace CryptoPuzzles.Server.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByLoginAsync(string login);
        Task<User?> GetByEmailAsync(string email);
    }
}