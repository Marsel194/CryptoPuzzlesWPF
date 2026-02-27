
using System.ComponentModel.DataAnnotations;

namespace CryptoPuzzles.Server.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public required string Login { get; set; }

        [MaxLength(128)]
        public required string PasswordHash { get; set; }

        [MaxLength(30)]
        public required string FirstName { get; set; }

        [MaxLength(30)]
        public required string LastName { get; set; }

        [MaxLength(30)]
        public string? MiddleName { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Puzzle> CreatedPuzzles { get; set; } = new List<Puzzle>();

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}