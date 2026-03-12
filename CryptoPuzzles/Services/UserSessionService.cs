namespace CryptoPuzzles.Services
{
    public class UserSessionService
    {
        public int? CurrentUserId { get; private set; }
        public string? CurrentUserLogin { get; private set; }
        public string? CurrentUserRole { get; private set; }
        public bool IsAuthenticated => CurrentUserId.HasValue;

        public void SetUser(int userId, string login, string role = "User", string username = null, bool isAdmin = false)
        {
            CurrentUserId = userId;
            CurrentUserLogin = login;
            CurrentUserRole = role;
        }

        public void ClearUser()
        {
            CurrentUserId = null;
            CurrentUserLogin = null;
            CurrentUserRole = null;
        }
    }
}