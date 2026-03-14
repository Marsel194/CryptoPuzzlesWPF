using System.Text.Json.Serialization;

namespace CryptoPuzzles.Server.Models
{
    public class SessionProgress
    {
        public int Id { get; set; }

        public int SessionId { get; set; }

        public int PuzzleId { get; set; }

        public int PuzzleOrder { get; set; }

        public bool Solved { get; set; } = false;

        public int HintsUsed { get; set; } = 0;

        public int ScoreEarned { get; set; } = 0;

        public DateTime StartedAt { get; set; }

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