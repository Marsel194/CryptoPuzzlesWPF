using System.ComponentModel.DataAnnotations.Schema;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Models
{
    public class GameSession
    {
        [ExportName("ID")]
        public int Id { get; set; }

        [ExportName("ID пользователя")]
        public int UserId { get; set; }

        [ExportName("Тип сессии")]
        public string SessionType { get; set; } = "training";

        [ExportName("Всего очков")]
        public int TotalScore { get; set; } = 0;

        [ExportName("Дата начала сессии")]
        public DateTime SessionStart { get; set; }

        [ExportName("Дата завершения")]
        public DateTime? CompletedAt { get; set; }

        [ExportName("Завершена")]
        public bool IsCompleted { get; set; } = false;

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