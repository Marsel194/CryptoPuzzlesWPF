using CryptoPuzzles.Shared;
using System.Text.Json.Serialization;

namespace CryptoPuzzles.Server.Models
{
    public class SessionProgress
    {
        [ExportName("ID")]
        public int Id { get; set; }

        [ExportName("ID сессии")]
        public int SessionId { get; set; }

        [ExportName("ID головоломки")]
        public int PuzzleId { get; set; }

        [ExportName("Порядковый номер")]
        public int PuzzleOrder { get; set; }

        [ExportName("Решено")]
        public bool Solved { get; set; } = false;

        [ExportName("Использовано подсказок")]
        public int HintsUsed { get; set; } = 0;

        [ExportName("Заработано очков")]
        public int ScoreEarned { get; set; } = 0;

        [ExportName("Начало решения")]
        public DateTime StartedAt { get; set; }

        [ExportName("Время решения")]
        public DateTime? SolvedAt { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        [JsonIgnore]
        public virtual GameSession Session { get; set; } = null!;

        [JsonIgnore]
        public virtual Puzzle Puzzle { get; set; } = null!;

        public TimeSpan? TimeToSolve => SolvedAt.HasValue
            ? SolvedAt.Value - StartedAt
            : null;
    }
}