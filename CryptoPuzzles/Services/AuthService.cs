using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Services
{
    public interface IAuthService
    {
        AAdmin? CurrentAdmin { get; set; }
        int? CurrentAdminId => CurrentAdmin?.Id;
        bool IsAuthenticated => CurrentAdmin != null;
        void Clear();
    }

    public class AuthService : IAuthService
    {
        public AAdmin? CurrentAdmin { get; set; }

        public void Clear()
        {
            CurrentAdmin = null;
        }
    }
}