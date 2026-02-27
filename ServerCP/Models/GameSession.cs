namespace CryptoPuzzles.Server.Models
{
    public class GameSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Score { get; set; } = 0;
        public DateTime SessionStartTime { get; set; }
        public int? CurrentPuzzleId { get; set; }
        public bool TrainingCompleted { get; set; } = false;
        public int HintsUsed { get; set; } = 0;
        public DateTime? CompletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Puzzle? CurrentPuzzle { get; set; }

        public TimeSpan? Duration => CompletedAt.HasValue
            ? CompletedAt.Value - SessionStartTime
            : null;
    }
}