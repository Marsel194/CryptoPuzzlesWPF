namespace CryptoPuzzles.SharedDTO
{
    public record AAdminDto(int Id, string Login, string FirstName, string LastName, string MiddleName, DateTime CreatedAt);
    public record UAEncryptionMethod(int Id);
    public record UAErrorResponse(string Message, string? Details);
    public record UAGameSession(int Id);
    public record UAHint(int Id);
    public record UALoginResponse(int Id, string Login, string Email, string Username, string Token);
    public record UAPuzzle(int Id);
    public record UAUser(int Id, string Login, string Username, string Email, string CreatedAt);
    public record UATutorial(int Id);
}
