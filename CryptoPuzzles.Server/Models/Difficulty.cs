using CryptoPuzzles.Shared;
using System.ComponentModel.DataAnnotations;

namespace CryptoPuzzles.Server.Models
{
    public class Difficulty : IEntityWithId, ISoftDelete
    {
        public int Id { get; set; }

        [MaxLength(30)]
        public string DifficultyName { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Puzzle> Puzzles { get; set; } = new List<Puzzle>();
    }
}