namespace CryptoPuzzles.SharedDTO
{
    // Admin
    public record AAdmin(int Id, string Login, string FirstName, string LastName, string? MiddleName, DateTime CreatedAt);
    public record AAdminCreate(string Login, string Password, string FirstName, string LastName, string? MiddleName);
    public record AAdminUpdate(int Id, string Login, string FirstName, string LastName, string? MiddleName, string? Password);

    // Difficulty
    public record ADifficulty(int Id, string DifficultyName);
    public record ADifficultyCreate(string DifficultyName);
    public record ADifficultyUpdate(int Id, string DifficultyName);

    // EncryptionMethod
    public record AEncryptionMethod(int Id, string Name);
    public record AEncryptionMethodCreate(string Name);
    public record AEncryptionMethodUpdate(int Id, string Name);

    // Puzzle
    public record APuzzle(int Id, string Title, string Content, string Answer, int MaxScore,
        int DifficultyId, string DifficultyName, int? MethodId, string? MethodName,
        bool IsTraining, int? TutorialOrder, int? CreatedByAdminId, DateTime CreatedAt);
    public record APuzzleCreate(string Title, string Content, string Answer, int MaxScore,
        int DifficultyId, int? MethodId, bool IsTraining, int? TutorialOrder);
    public record APuzzleUpdate(int Id, string Title, string Content, string Answer, int MaxScore,
        int DifficultyId, int? MethodId, bool IsTraining, int? TutorialOrder);

    // Hint
    public record AHint(int Id, int PuzzleId, string PuzzleTitle, string HintText, int HintOrder, DateTime CreatedAt);
    public record AHintCreate(int PuzzleId, string HintText, int HintOrder);
    public record AHintUpdate(int Id, int PuzzleId, string HintText, int HintOrder);

    // GameSession
    public record AGameSession(int Id, int UserId, string UserLogin, int Score, DateTime SessionStartTime,
        int? CurrentPuzzleId, string? CurrentPuzzleTitle, bool TrainingCompleted, int HintsUsed, DateTime? CompletedAt);
    public record AGameSessionUpdate(int Id, int Score, int? CurrentPuzzleId, bool TrainingCompleted, int HintsUsed, DateTime? CompletedAt);

    // Tutorial
    public record ATutorial(int Id, int MethodId, string MethodName, string TheoryTitle,
        string TheoryContent, int SortOrder, bool IsActive, DateTime CreatedAt, DateTime UpdatedAt);
    public record ATutorialCreate(int MethodId, string TheoryTitle, string TheoryContent, int SortOrder, bool IsActive);
    public record ATutorialUpdate(int Id, int MethodId, string TheoryTitle, string TheoryContent, int SortOrder, bool IsActive);

    // User
    public record AUser(int Id, string Login, string Username, string Email, DateTime? CreatedAt);
    public record AUserUpdate(int Id, string Login, string Username, string Email);

    // Auth / Common
    public record UAErrorResponse(string Message, string? Details);
    public record UALoginResponse(string Login, string Email, string Username, bool IsAdmin);
    public record UALoginRequest(string Login, string Password);
    public record UARegisterRequest(string Login, string Username, string Email, string Password);
    public record UARegisterResponse(int Id, string Login, string Username, string Email);
}