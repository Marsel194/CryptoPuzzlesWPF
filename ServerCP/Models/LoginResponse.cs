namespace CryptoPuzzles.Server.Models
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string? Email { get; set; }
        public string Token { get; set; }
        public bool IsAdmin { get; set; }
    }
}
