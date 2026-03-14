using System.ComponentModel.DataAnnotations;

namespace CryptoPuzzles.Server.Models
{
    public class EncryptionMethod
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public required string Name { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Puzzle> Puzzles { get; set; } = [];
        public virtual ICollection<Tutorial> Tutorials { get; set; } = [];
    }
}