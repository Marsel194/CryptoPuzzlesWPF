using System.ComponentModel.DataAnnotations;

namespace CryptoPuzzles.Server.Models
{
    public class User
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public required string Login { get; set; }

        [MaxLength(128)]
        public required string PasswordHash { get; set; }

        [MaxLength(150), EmailAddress]
        public required string Email { get; set; }

        [MaxLength(60)]
        public required string Username { get; set; }

        public DateTime? CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<GameSession> GameSessions { get; set; } = [];

        public virtual UserStatistic? Statistic { get; set; }
    }
}