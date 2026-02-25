namespace CryptoPuzzles.SharedDTO
{
    public record AAdminDto(int Id, string Login, string FirstName, string LastName, string MiddleName, DateTime CreatedAt);
    public record UAEncryptionMethod(int Id);
    public record UAErrorResponse(string Message, string? Details);
    public record UAGameSession(int Id);
    public record UAHint(int Id);
    public record UALoginResponse(int Id, string Login, string Email, string Username, string Token);
    public record UALoginRequest(string Login, string Password);
    public record UAPuzzle(int Id);
    public record AUser(int Id, string Login, string Username, string Email, DateTime CreatedAt);
    public record UUser(string Login, string Username, string Email);
    public record UATutorial(int Id);
}
