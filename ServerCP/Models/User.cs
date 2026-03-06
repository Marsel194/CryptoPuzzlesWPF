using CryptoPuzzles.Shared;
using System.ComponentModel.DataAnnotations;

namespace CryptoPuzzles.Server.Models
{
    public class User
    {
        [ExportName("ID")]
        public int Id { get; set; }

        [ExportName("Логин")]
        [MaxLength(50)]
        public required string Login { get; set; }

        [MaxLength(128)]
        public required string PasswordHash { get; set; }

        [ExportName("Почта")]
        [MaxLength(150), EmailAddress]
        public required string Email { get; set; }

        [ExportName("Имя пользователя")]
        [MaxLength(60)]
        public required string Username { get; set; }

        [ExportName("Дата создания")]
        public DateTime? CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<GameSession> GameSessions { get; set; } = [];
    }
}