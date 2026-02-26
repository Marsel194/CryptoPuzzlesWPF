namespace CryptoPuzzles.Server.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public required string Login { get; set; }
        public required string PasswordHash { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? MiddleName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
