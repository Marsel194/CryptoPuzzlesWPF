using CryptoPuzzles.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoPuzzles.Server.Models
{
    public class SessionProgress : IEntityWithId, ISoftDelete
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int PuzzleId { get; set; }
        public int PuzzleOrder { get; set; }
        public bool Solved { get; set; }
        public int HintsUsed { get; set; }
        public int ScoreEarned { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SolvedAt { get; set; }
        public TimeSpan? TimeToSolve { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(SessionId))]
        public virtual GameSession Session { get; set; } = null!;
        [ForeignKey(nameof(PuzzleId))]
        public virtual Puzzle Puzzle { get; set; } = null!;
    }
}