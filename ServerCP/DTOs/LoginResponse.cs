namespace CryptoPuzzles.Server.DTOs
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public required string Login { get; set; }
        public string? Email { get; set; }
        public required string Username { get; set; }
        public bool IsAdmin { get; set; }
    }
}
