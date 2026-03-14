using CryptoPuzzles.Shared;
using System.ComponentModel.DataAnnotations;

namespace CryptoPuzzles.Server.Models
{
    public class Admin : IEntityWithId, ISoftDelete, IHasCreatedAt
    {
        public Admin() { }

        public int Id { get; set; }

        [MaxLength(50)]
        public string Login { get; set; } = string.Empty;

        [MaxLength(128)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(30)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(30)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? MiddleName { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Puzzle> CreatedPuzzles { get; set; } = [];

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}