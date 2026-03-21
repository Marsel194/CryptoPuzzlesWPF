using System.Diagnostics;

namespace CryptoPuzzles.Client.Services
{
    public class UserSessionService
    {
        private int? _currentUserId;
        private string? _currentUsername;
        private bool _isAdmin;
        private string? _token;

        public int? CurrentUserId => _currentUserId;
        public string? CurrentUsername => _currentUsername;
        public bool IsAdmin => _isAdmin;
        public string? Token => _token;
        public bool IsAuthenticated => _currentUserId.HasValue && !string.IsNullOrEmpty(_token);

        public void SetUser(int userId, string login, string username, bool isAdmin, string token)
        {
            Debug.WriteLine($"SetUser called - UserId: {userId}, IsAdmin: {isAdmin}, Token: {(string.IsNullOrEmpty(token) ? "NULL" : "PRESENT")}");

            _currentUserId = userId;
            _currentUsername = username;
            _isAdmin = isAdmin;
            _token = token;

            Debug.WriteLine($"Token stored: {(_token != null ? "Yes" : "No")}");
        }

        public void ClearUser()
        {
            Debug.WriteLine("ClearUser called");
            _currentUserId = null;
            _currentUsername = null;
            _isAdmin = false;
            _token = null;
        }
    }
}