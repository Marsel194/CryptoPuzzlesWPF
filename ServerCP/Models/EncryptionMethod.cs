using System.ComponentModel.DataAnnotations;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Models
{
    public class EncryptionMethod : IEntityWithId, ISoftDelete
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Puzzle> Puzzles { get; set; } = new List<Puzzle>();
        public virtual ICollection<Tutorial> Tutorials { get; set; } = new List<Tutorial>();
    }
}