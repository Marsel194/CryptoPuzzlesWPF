namespace Hairulin_02_01
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string? PasswordHash { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
