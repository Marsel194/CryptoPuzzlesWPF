using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Models
{
    public class GameSession
    {
        [ExportName("ID")]
        public int Id { get; set; }

        [ExportName("ID пользователя")]
        public int UserId { get; set; }

        [ExportName("Очки")]
        public int Score { get; set; } = 0;

        [ExportName("Дата начала сессии")]
        public DateTime SessionStartTime { get; set; }

        [ExportName("Текущий пазл")]
        public int? CurrentPuzzleId { get; set; }

        [ExportName("Тренировка завершена")]
        public bool TrainingCompleted { get; set; } = false;

        [ExportName("Использовано подсказок")]
        public int HintsUsed { get; set; } = 0;

        [ExportName("Дата завершения сессии")]
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