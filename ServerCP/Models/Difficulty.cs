using System.ComponentModel.DataAnnotations;

namespace CryptoPuzzles.Server.Models
{
    public class Difficulty
    {
        public int Id { get; set; }

        [MaxLength(30)]
        public required string DifficultyName { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Puzzle> Puzzles { get; set; } = new List<Puzzle>();
    }
}