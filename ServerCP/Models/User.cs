using CryptoPuzzles.Server.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Models
{
    public class User : IEntityWithId, ISoftDelete
    {
        public int Id { get; set; }

        [MaxLength(30)]
        public string Login { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Username { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        [JsonIgnore]
        public virtual ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();

        [JsonIgnore]
        public virtual UserStatistic? Statistic { get; set; }
    }
}