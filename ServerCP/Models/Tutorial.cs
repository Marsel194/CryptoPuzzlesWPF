using System.ComponentModel.DataAnnotations;

namespace CryptoPuzzles.Server.Models
{
    public class Tutorial
    {
        public int Id { get; set; }
        public int MethodId { get; set; }

        [MaxLength(150)]
        public required string TheoryTitle { get; set; }

        public required string TheoryContent { get; set; }

        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual EncryptionMethod Method { get; set; } = null!;
    }
}