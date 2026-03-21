using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Client.Services
{
    public interface IAuthService
    {
        AAdmin? CurrentAdmin { get; }
        int? CurrentAdminId => CurrentAdmin?.Id;
        bool IsAuthenticated => CurrentAdmin != null;
        void SetCurrentAdmin(AAdmin admin);
        void Clear();
    }

    public class AuthService : IAuthService
    {
        public AAdmin? CurrentAdmin { get; private set; }

        public void SetCurrentAdmin(AAdmin admin)
        {
            CurrentAdmin = admin;
        }

        public void Clear()
        {
            CurrentAdmin = null;
        }
    }
}