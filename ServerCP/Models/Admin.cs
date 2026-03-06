using CryptoPuzzles.Shared;
using System.ComponentModel.DataAnnotations;

namespace CryptoPuzzles.Server.Models
{
    public class Admin
    {
        [ExportName("ID")]
        public int Id { get; set; }

        [ExportName("Логин")]
        [MaxLength(50)]
        public required string Login { get; set; }

        [MaxLength(128)]
        public required string PasswordHash { get; set; }

        [ExportName("Имя")]
        [MaxLength(30)]
        public required string FirstName { get; set; }

        [ExportName("Фамилия")]
        [MaxLength(30)]
        public required string LastName { get; set; }

        [ExportName("Отчество")]
        [MaxLength(30)]
        public string? MiddleName { get; set; }

        [ExportName("Дата создания")]
        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Puzzle> CreatedPuzzles { get; set; } = [];

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}