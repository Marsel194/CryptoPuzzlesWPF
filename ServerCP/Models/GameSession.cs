using System.ComponentModel.DataAnnotations.Schema;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Models
{
    public class GameSession : IEntityWithId, ISoftDelete
    {
        public GameSession() { }

        public int Id { get; set; }

        public int UserId { get; set; }

        public string SessionType { get; set; } = "training";

        public int TotalScore { get; set; } = 0;

        public DateTime SessionStart { get; set; }

        public DateTime? CompletedAt { get; set; }

        public bool IsCompleted { get; set; } = false;

        public int? CurrentTutorialIndex { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<SessionProgress> Progresses { get; set; } = new List<SessionProgress>();

        [NotMapped]
        public int PuzzlesCount => Progresses?.Count ?? 0;

        [NotMapped]
        public int SolvedCount => Progresses?.Count(p => p.Solved) ?? 0;

        [NotMapped]
        public TimeSpan? Duration => CompletedAt.HasValue
            ? CompletedAt.Value - SessionStart
            : null;
    }
}