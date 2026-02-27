namespace CryptoPuzzles.Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Username { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<GameSession> GameSessions { get; set; } = [];
    }
}