using System;

namespace CryptoPuzzles.SharedDTO
{
    // ---- Изменяемые классы для отображения и редактирования в DataGrid ----

    public class AAdmin
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public DateTime CreatedAt { get; set; }

        public AAdmin(int id, string login, string firstName, string lastName, string? middleName, DateTime createdAt)
        {
            Id = id;
            Login = login;
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            CreatedAt = createdAt;
        }
        public AAdmin() { }
    }

    public class ADifficulty
    {
        public int Id { get; set; }
        public string DifficultyName { get; set; }

        public ADifficulty(int id, string difficultyName)
        {
            Id = id;
            DifficultyName = difficultyName;
        }
        public ADifficulty() { }
    }

    public class AEncryptionMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public AEncryptionMethod(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public AEncryptionMethod() { }
    }

    public class APuzzle
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Answer { get; set; }
        public int MaxScore { get; set; }
        public int DifficultyId { get; set; }
        public string DifficultyName { get; set; }
        public int? MethodId { get; set; }
        public string? MethodName { get; set; }
        public bool IsTraining { get; set; }
        public int? TutorialOrder { get; set; }
        public int? CreatedByAdminId { get; set; }
        public DateTime CreatedAt { get; set; }

        public APuzzle(int id, string title, string content, string answer, int maxScore,
            int difficultyId, string difficultyName, int? methodId, string? methodName,
            bool isTraining, int? tutorialOrder, int? createdByAdminId, DateTime createdAt)
        {
            Id = id;
            Title = title;
            Content = content;
            Answer = answer;
            MaxScore = maxScore;
            DifficultyId = difficultyId;
            DifficultyName = difficultyName;
            MethodId = methodId;
            MethodName = methodName;
            IsTraining = isTraining;
            TutorialOrder = tutorialOrder;
            CreatedByAdminId = createdByAdminId;
            CreatedAt = createdAt;
        }
        public APuzzle() { }
    }

    public class AHint
    {
        public int Id { get; set; }
        public int PuzzleId { get; set; }
        public string PuzzleTitle { get; set; }
        public string HintText { get; set; }
        public int HintOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        public AHint(int id, int puzzleId, string puzzleTitle, string hintText, int hintOrder, DateTime createdAt)
        {
            Id = id;
            PuzzleId = puzzleId;
            PuzzleTitle = puzzleTitle;
            HintText = hintText;
            HintOrder = hintOrder;
            CreatedAt = createdAt;
        }
        public AHint() { }
    }

    public class AGameSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserLogin { get; set; }
        public int Score { get; set; }
        public DateTime SessionStartTime { get; set; }
        public int? CurrentPuzzleId { get; set; }
        public string? CurrentPuzzleTitle { get; set; }
        public bool TrainingCompleted { get; set; }
        public int HintsUsed { get; set; }
        public DateTime? CompletedAt { get; set; }

        public AGameSession(int id, int userId, string userLogin, int score, DateTime sessionStartTime,
            int? currentPuzzleId, string? currentPuzzleTitle, bool trainingCompleted, int hintsUsed, DateTime? completedAt)
        {
            Id = id;
            UserId = userId;
            UserLogin = userLogin;
            Score = score;
            SessionStartTime = sessionStartTime;
            CurrentPuzzleId = currentPuzzleId;
            CurrentPuzzleTitle = currentPuzzleTitle;
            TrainingCompleted = trainingCompleted;
            HintsUsed = hintsUsed;
            CompletedAt = completedAt;
        }
        public AGameSession() { }
    }

    public class ATutorial
    {
        public int Id { get; set; }
        public int MethodId { get; set; }
        public string MethodName { get; set; }
        public string TheoryTitle { get; set; }
        public string TheoryContent { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        public ATutorial(int id, int methodId, string methodName, string theoryTitle, string theoryContent, int sortOrder, DateTime createdAt)
        {
            Id = id;
            MethodId = methodId;
            MethodName = methodName;
            TheoryTitle = theoryTitle;
            TheoryContent = theoryContent;
            SortOrder = sortOrder;
            CreatedAt = createdAt;
        }
        public ATutorial() { }
    }

    public class AUser
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime? CreatedAt { get; set; }

        public AUser(int id, string login, string username, string email, DateTime? createdAt)
        {
            Id = id;
            Login = login;
            Username = username;
            Email = email;
            CreatedAt = createdAt;
        }
        public AUser() { }
    }

    // ---- Неизменяемые record'ы для Create/Update операций (не используются в DataGrid) ----
    public record AAdminCreate(string Login, string Password, string FirstName, string LastName, string? MiddleName);
    public record AAdminUpdate(int Id, string Login, string FirstName, string LastName, string? MiddleName, string? Password);
    public record ADifficultyCreate(string DifficultyName);
    public record ADifficultyUpdate(int Id, string DifficultyName);
    public record AEncryptionMethodCreate(string Name);
    public record AEncryptionMethodUpdate(int Id, string Name);
    public record APuzzleCreate(string Title, string Content, string Answer, int MaxScore,
        int DifficultyId, int? MethodId, bool IsTraining, int? TutorialOrder);
    public record APuzzleUpdate(int Id, string Title, string Content, string Answer, int MaxScore,
        int DifficultyId, int? MethodId, bool IsTraining, int? TutorialOrder);
    public record AHintCreate(int PuzzleId, string HintText, int HintOrder);
    public record AHintUpdate(int Id, int PuzzleId, string HintText, int HintOrder);
    public record AGameSessionCreate(int UserId, int Score, int? CurrentPuzzleId, bool TrainingCompleted, int HintsUsed, DateTime? CompletedAt);
    public record AGameSessionUpdate(int Id, int? Score, int? CurrentPuzzleId, bool? TrainingCompleted, int? HintsUsed, DateTime? CompletedAt);
    public record ATutorialCreate(int MethodId, string TheoryTitle, string TheoryContent, int SortOrder);
    public record ATutorialUpdate(int Id, int MethodId, string TheoryTitle, string TheoryContent, int SortOrder);
    public record AUserCreate(string Login, string Password, string Username, string Email);
    public record AUserUpdate(int Id, string Login, string Username, string Email, string? Password = null);
    public record UAErrorResponse(string Message, string? Details);
    public record UALoginResponse(string Login, string Email, string Username, bool IsAdmin);
    public record UALoginRequest(string Login, string Password);
    public record UARegisterRequest(string Login, string Username, string Email, string Password);
    public record UARegisterResponse(int Id, string Login, string Username, string Email);
}