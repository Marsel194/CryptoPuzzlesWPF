using CryptoPuzzles.Shared;
using System.ComponentModel.DataAnnotations;

namespace CryptoPuzzles.Server.Models
{
    public class Puzzle
    {
        [ExportName("ID")]
        public int Id { get; set; }

        [ExportName("Название пазла")]
        [MaxLength(100)]
        public required string Title { get; set; }

        [ExportName("Содержание пазла")]
        public required string Content { get; set; }

        [ExportName("Ответ")]
        [MaxLength(100)]
        public required string Answer { get; set; }

        [ExportName("Максимальные очки")]
        public int MaxScore { get; set; } = 50;

        [ExportName("ID сложности")]
        public int DifficultyId { get; set; }

        [ExportName("ID метода")]
        public int? MethodId { get; set; }

        [ExportName("Обучение")]
        public bool IsTraining { get; set; }

        [ExportName("Порядок обучения")]
        public int? TutorialOrder { get; set; }

        [ExportName("Кто создал")]
        public int? CreatedByAdminId { get; set; }

        [ExportName("Дата создания")]
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Difficulty Difficulty { get; set; } = null!;
        public virtual EncryptionMethod? Method { get; set; }
        public virtual Admin? CreatedByAdmin { get; set; }
        public virtual ICollection<Hint> Hints { get; set; } = [];
        public virtual ICollection<GameSession> GameSessions { get; set; } = [];

        public virtual ICollection<SessionProgress> SessionProgresses { get; set; } = new List<SessionProgress>();
    }
}