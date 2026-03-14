using System.ComponentModel.DataAnnotations;

namespace CryptoPuzzles.Server.Models
{
    public class Puzzle
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public required string Title { get; set; }

        public required string Content { get; set; }

        [MaxLength(100)]
        public required string Answer { get; set; }

        public int MaxScore { get; set; } = 50;

        public int DifficultyId { get; set; }

        public int? MethodId { get; set; }

        public bool IsTraining { get; set; }

        public int? TutorialOrder { get; set; }

        public int? CreatedByAdminId { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Difficulty Difficulty { get; set; } = null!;
        public virtual EncryptionMethod? Method { get; set; }
        public virtual Admin? CreatedByAdmin { get; set; }
        public virtual ICollection<Hint> Hints { get; set; } = [];

        public virtual ICollection<SessionProgress> SessionProgresses { get; set; } = new List<SessionProgress>();
    }
}