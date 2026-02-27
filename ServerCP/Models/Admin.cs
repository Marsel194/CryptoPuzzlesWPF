
namespace CryptoPuzzles.Server.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Puzzle> CreatedPuzzles { get; set; } = [];

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}