using CryptoPuzzles.Shared;
using System.ComponentModel.DataAnnotations;

namespace CryptoPuzzles.Server.Models
{
    public class Puzzle : IEntityWithId, ISoftDelete, IHasCreatedAt
    {
        public Puzzle() { }

        public int Id { get; set; }

        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Answer { get; set; } = string.Empty;

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