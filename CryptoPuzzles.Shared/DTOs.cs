using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CryptoPuzzles.Shared
{
    public class AAdmin : INotifyPropertyChanged
    {
        private int _id;
        private string _login = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string? _middleName;
        private DateTime _createdAt;
        private bool _isDeleted;
        private DateTime? _deletedAt;

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string Login { get => _login; set { _login = value; OnPropertyChanged(); } }
        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(); } }
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(); } }
        public string? MiddleName { get => _middleName; set { _middleName = value; OnPropertyChanged(); } }
        public DateTime CreatedAt { get => _createdAt; set { _createdAt = value; OnPropertyChanged(); } }
        public bool IsDeleted { get => _isDeleted; set { _isDeleted = value; OnPropertyChanged(); } }
        public DateTime? DeletedAt { get => _deletedAt; set { _deletedAt = value; OnPropertyChanged(); } }

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();

        public AAdmin(int id, string login, string firstName, string lastName, string? middleName,
                     DateTime createdAt, bool isDeleted = false, DateTime? deletedAt = null)
        {
            _id = id;
            _login = login;
            _firstName = firstName;
            _lastName = lastName;
            _middleName = middleName;
            _createdAt = createdAt;
            _isDeleted = isDeleted;
            _deletedAt = deletedAt;
        }
        public AAdmin() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class ADifficulty : INotifyPropertyChanged
    {
        private int _id;
        private string _difficultyName = string.Empty;
        private bool _isDeleted;
        private DateTime? _deletedAt;

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string DifficultyName { get => _difficultyName; set { _difficultyName = value; OnPropertyChanged(); } }
        public bool IsDeleted { get => _isDeleted; set { _isDeleted = value; OnPropertyChanged(); } }
        public DateTime? DeletedAt { get => _deletedAt; set { _deletedAt = value; OnPropertyChanged(); } }

        public ADifficulty(int id, string difficultyName, bool isDeleted = false, DateTime? deletedAt = null)
        {
            _id = id;
            _difficultyName = difficultyName;
            _isDeleted = isDeleted;
            _deletedAt = deletedAt;
        }
        public ADifficulty() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class AEncryptionMethod : INotifyPropertyChanged
    {
        private int _id;
        private string _name = string.Empty;
        private bool _isDeleted;
        private DateTime? _deletedAt;

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public bool IsDeleted { get => _isDeleted; set { _isDeleted = value; OnPropertyChanged(); } }
        public DateTime? DeletedAt { get => _deletedAt; set { _deletedAt = value; OnPropertyChanged(); } }

        public AEncryptionMethod(int id, string name, bool isDeleted = false, DateTime? deletedAt = null)
        {
            _id = id;
            _name = name;
            _isDeleted = isDeleted;
            _deletedAt = deletedAt;
        }
        public AEncryptionMethod() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class APuzzle : INotifyPropertyChanged
    {
        private int _id;
        private string _title = string.Empty;
        private string _content = string.Empty;
        private string _answer = string.Empty;
        private int _maxScore;
        private int _difficultyId;
        private string _difficultyName = string.Empty;
        private int? _methodId;
        private string? _methodName;
        private bool _isTraining;
        private int? _tutorialOrder;
        private int? _createdByAdminId;
        private string? _createdByAdminName;
        private DateTime _createdAt;
        private bool _isDeleted;
        private DateTime? _deletedAt;

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string Title { get => _title; set { _title = value; OnPropertyChanged(); } }
        public string Content { get => _content; set { _content = value; OnPropertyChanged(); } }
        public string Answer { get => _answer; set { _answer = value; OnPropertyChanged(); } }
        public int MaxScore { get => _maxScore; set { _maxScore = value; OnPropertyChanged(); } }
        public int DifficultyId { get => _difficultyId; set { _difficultyId = value; OnPropertyChanged(); } }
        public string DifficultyName { get => _difficultyName; set { _difficultyName = value; OnPropertyChanged(); } }
        public int? MethodId { get => _methodId; set { _methodId = value; OnPropertyChanged(); } }
        public string? MethodName { get => _methodName; set { _methodName = value; OnPropertyChanged(); } }
        public bool IsTraining { get => _isTraining; set { _isTraining = value; OnPropertyChanged(); } }
        public int? TutorialOrder { get => _tutorialOrder; set { _tutorialOrder = value; OnPropertyChanged(); } }
        public int? CreatedByAdminId { get => _createdByAdminId; set { _createdByAdminId = value; OnPropertyChanged(); } }
        public string? CreatedByAdminName { get => _createdByAdminName; set { _createdByAdminName = value; OnPropertyChanged(); } }
        public DateTime CreatedAt { get => _createdAt; set { _createdAt = value; OnPropertyChanged(); } }
        public bool IsDeleted { get => _isDeleted; set { _isDeleted = value; OnPropertyChanged(); } }
        public DateTime? DeletedAt { get => _deletedAt; set { _deletedAt = value; OnPropertyChanged(); } }

        public string PuzzleType => IsTraining ? "Обучение" : "Практика";
        public string HasMethod => MethodId.HasValue ? "Да" : "Нет";

        public APuzzle(int id, string title, string content, string answer, int maxScore,
            int difficultyId, string difficultyName, int? methodId, string? methodName,
            bool isTraining, int? tutorialOrder, int? createdByAdminId, string? createdByAdminName,
            DateTime createdAt, bool isDeleted = false, DateTime? deletedAt = null)
        {
            _id = id;
            _title = title;
            _content = content;
            _answer = answer;
            _maxScore = maxScore;
            _difficultyId = difficultyId;
            _difficultyName = difficultyName;
            _methodId = methodId;
            _methodName = methodName;
            _isTraining = isTraining;
            _tutorialOrder = tutorialOrder;
            _createdByAdminId = createdByAdminId;
            _createdByAdminName = createdByAdminName;
            _createdAt = createdAt;
            _isDeleted = isDeleted;
            _deletedAt = deletedAt;
        }
        public APuzzle() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class AHint : INotifyPropertyChanged
    {
        private int _id;
        private int _puzzleId;
        private string _puzzleTitle = string.Empty;
        private string _hintText = string.Empty;
        private int _hintOrder;
        private DateTime _createdAt;
        private bool _isDeleted;
        private DateTime? _deletedAt;

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public int PuzzleId { get => _puzzleId; set { _puzzleId = value; OnPropertyChanged(); } }
        public string PuzzleTitle { get => _puzzleTitle; set { _puzzleTitle = value; OnPropertyChanged(); } }
        public string HintText { get => _hintText; set { _hintText = value; OnPropertyChanged(); } }
        public int HintOrder { get => _hintOrder; set { _hintOrder = value; OnPropertyChanged(); } }
        public DateTime CreatedAt { get => _createdAt; set { _createdAt = value; OnPropertyChanged(); } }
        public bool IsDeleted { get => _isDeleted; set { _isDeleted = value; OnPropertyChanged(); } }
        public DateTime? DeletedAt { get => _deletedAt; set { _deletedAt = value; OnPropertyChanged(); } }

        public AHint(int id, int puzzleId, string puzzleTitle, string hintText, int hintOrder,
                    DateTime createdAt, bool isDeleted = false, DateTime? deletedAt = null)
        {
            _id = id;
            _puzzleId = puzzleId;
            _puzzleTitle = puzzleTitle;
            _hintText = hintText;
            _hintOrder = hintOrder;
            _createdAt = createdAt;
            _isDeleted = isDeleted;
            _deletedAt = deletedAt;
        }
        public AHint() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class AGameSession : INotifyPropertyChanged
    {
        private int _id;
        private int _userId;
        private string _userLogin = string.Empty;
        private string _username = string.Empty;
        private string _sessionType = string.Empty;
        private int _totalScore;
        private DateTime _sessionStart;
        private DateTime? _completedAt;
        private bool _isCompleted;
        private bool _isDeleted;
        private DateTime? _deletedAt;
        private int _puzzlesCount;
        private int _solvedCount;
        private int? _currentTutorialIndex;

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public int UserId { get => _userId; set { _userId = value; OnPropertyChanged(); } }
        public string UserLogin { get => _userLogin; set { _userLogin = value; OnPropertyChanged(); } }
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public string SessionType { get => _sessionType; set { _sessionType = value; OnPropertyChanged(); } }
        public int TotalScore { get => _totalScore; set { _totalScore = value; OnPropertyChanged(); } }
        public DateTime SessionStart { get => _sessionStart; set { _sessionStart = value; OnPropertyChanged(); } }
        public DateTime? CompletedAt { get => _completedAt; set { _completedAt = value; OnPropertyChanged(); } }
        public bool IsCompleted { get => _isCompleted; set { _isCompleted = value; OnPropertyChanged(); } }
        public bool IsDeleted { get => _isDeleted; set { _isDeleted = value; OnPropertyChanged(); } }
        public DateTime? DeletedAt { get => _deletedAt; set { _deletedAt = value; OnPropertyChanged(); } }
        public int PuzzlesCount { get => _puzzlesCount; set { _puzzlesCount = value; OnPropertyChanged(); } }
        public int SolvedCount { get => _solvedCount; set { _solvedCount = value; OnPropertyChanged(); } }
        public int? CurrentTutorialIndex { get => _currentTutorialIndex; set { _currentTutorialIndex = value; OnPropertyChanged(); } }

        public string Status => IsCompleted ? "Завершена" : "Активна";
        public string Duration => CompletedAt.HasValue ? (CompletedAt.Value - SessionStart).ToString(@"hh\:mm\:ss") : "—";
        public double Progress => PuzzlesCount > 0 ? (double)SolvedCount / PuzzlesCount * 100 : 0;

        public AGameSession(int id, int userId, string userLogin, string username, string sessionType,
            int totalScore, DateTime sessionStart, DateTime? completedAt, bool isCompleted,
            int puzzlesCount, int solvedCount, int? currentTutorialIndex,
            bool isDeleted = false, DateTime? deletedAt = null)
        {
            _id = id;
            _userId = userId;
            _userLogin = userLogin;
            _username = username;
            _sessionType = sessionType;
            _totalScore = totalScore;
            _sessionStart = sessionStart;
            _completedAt = completedAt;
            _isCompleted = isCompleted;
            _puzzlesCount = puzzlesCount;
            _solvedCount = solvedCount;
            _currentTutorialIndex = currentTutorialIndex;
            _isDeleted = isDeleted;
            _deletedAt = deletedAt;
        }
        public AGameSession() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class ATutorial : INotifyPropertyChanged
    {
        private int _id;
        private int _methodId;
        private string _methodName = string.Empty;
        private string _theoryTitle = string.Empty;
        private string _theoryContent = string.Empty;
        private int _sortOrder;
        private DateTime _createdAt;
        private bool _isDeleted;
        private DateTime? _deletedAt;

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public int MethodId { get => _methodId; set { _methodId = value; OnPropertyChanged(); } }
        public string MethodName { get => _methodName; set { _methodName = value; OnPropertyChanged(); } }
        public string TheoryTitle { get => _theoryTitle; set { _theoryTitle = value; OnPropertyChanged(); } }
        public string TheoryContent { get => _theoryContent; set { _theoryContent = value; OnPropertyChanged(); } }
        public int SortOrder { get => _sortOrder; set { _sortOrder = value; OnPropertyChanged(); } }
        public DateTime CreatedAt { get => _createdAt; set { _createdAt = value; OnPropertyChanged(); } }
        public bool IsDeleted { get => _isDeleted; set { _isDeleted = value; OnPropertyChanged(); } }
        public DateTime? DeletedAt { get => _deletedAt; set { _deletedAt = value; OnPropertyChanged(); } }

        public ATutorial(int id, int methodId, string methodName, string theoryTitle,
                        string theoryContent, int sortOrder, DateTime createdAt,
                        bool isDeleted = false, DateTime? deletedAt = null)
        {
            _id = id;
            _methodId = methodId;
            _methodName = methodName;
            _theoryTitle = theoryTitle;
            _theoryContent = theoryContent;
            _sortOrder = sortOrder;
            _createdAt = createdAt;
            _isDeleted = isDeleted;
            _deletedAt = deletedAt;
        }
        public ATutorial() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class AUser : INotifyPropertyChanged
    {
        private int _id;
        private string _login = string.Empty;
        private string _username = string.Empty;
        private string _email = string.Empty;
        private DateTime? _createdAt;
        private bool _isDeleted;
        private DateTime? _deletedAt;
        private int _totalSessions;
        private int _totalScore;
        private int _puzzlesSolved;

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string Login { get => _login; set { _login = value; OnPropertyChanged(); } }
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        public DateTime? CreatedAt { get => _createdAt; set { _createdAt = value; OnPropertyChanged(); } }
        public bool IsDeleted { get => _isDeleted; set { _isDeleted = value; OnPropertyChanged(); } }
        public DateTime? DeletedAt { get => _deletedAt; set { _deletedAt = value; OnPropertyChanged(); } }
        public int TotalSessions { get => _totalSessions; set { _totalSessions = value; OnPropertyChanged(); } }
        public int TotalScore { get => _totalScore; set { _totalScore = value; OnPropertyChanged(); } }
        public int PuzzlesSolved { get => _puzzlesSolved; set { _puzzlesSolved = value; OnPropertyChanged(); } }

        public AUser(int id, string login, string username, string email, DateTime? createdAt,
                    bool isDeleted = false, DateTime? deletedAt = null,
                    int totalSessions = 0, int totalScore = 0, int puzzlesSolved = 0)
        {
            _id = id;
            _login = login;
            _username = username;
            _email = email;
            _createdAt = createdAt;
            _isDeleted = isDeleted;
            _deletedAt = deletedAt;
            _totalSessions = totalSessions;
            _totalScore = totalScore;
            _puzzlesSolved = puzzlesSolved;
        }
        public AUser() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class ASessionProgress : INotifyPropertyChanged
    {
        private int _id;
        private int _sessionId;
        private string _userLogin = string.Empty;
        private string _username = string.Empty;
        private int _puzzleId;
        private string _puzzleTitle = string.Empty;
        private int _puzzleOrder;
        private bool _solved;
        private int _hintsUsed;
        private int _scoreEarned;
        private DateTime _startedAt;
        private DateTime? _solvedAt;
        private TimeSpan? _timeToSolve;
        private bool _isDeleted;
        private DateTime? _deletedAt;

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public int SessionId { get => _sessionId; set { _sessionId = value; OnPropertyChanged(); } }
        public string UserLogin { get => _userLogin; set { _userLogin = value; OnPropertyChanged(); } }
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public int PuzzleId { get => _puzzleId; set { _puzzleId = value; OnPropertyChanged(); } }
        public string PuzzleTitle { get => _puzzleTitle; set { _puzzleTitle = value; OnPropertyChanged(); } }
        public int PuzzleOrder { get => _puzzleOrder; set { _puzzleOrder = value; OnPropertyChanged(); } }
        public bool Solved { get => _solved; set { _solved = value; OnPropertyChanged(); } }
        public int HintsUsed { get => _hintsUsed; set { _hintsUsed = value; OnPropertyChanged(); } }
        public int ScoreEarned { get => _scoreEarned; set { _scoreEarned = value; OnPropertyChanged(); } }
        public DateTime StartedAt { get => _startedAt; set { _startedAt = value; OnPropertyChanged(); } }
        public DateTime? SolvedAt { get => _solvedAt; set { _solvedAt = value; OnPropertyChanged(); } }
        public TimeSpan? TimeToSolve { get => _timeToSolve; set { _timeToSolve = value; OnPropertyChanged(); } }
        public bool IsDeleted { get => _isDeleted; set { _isDeleted = value; OnPropertyChanged(); } }
        public DateTime? DeletedAt { get => _deletedAt; set { _deletedAt = value; OnPropertyChanged(); } }

        public string Status => Solved ? "Решено" : "Не решено";
        public string TimeSpent => TimeToSolve?.ToString(@"hh\:mm\:ss") ?? "—";

        public ASessionProgress(int id, int sessionId, string userLogin, string username,
            int puzzleId, string puzzleTitle, int puzzleOrder, bool solved,
            int hintsUsed, int scoreEarned, DateTime startedAt, DateTime? solvedAt,
            TimeSpan? timeToSolve, bool isDeleted = false, DateTime? deletedAt = null)
        {
            _id = id;
            _sessionId = sessionId;
            _userLogin = userLogin;
            _username = username;
            _puzzleId = puzzleId;
            _puzzleTitle = puzzleTitle;
            _puzzleOrder = puzzleOrder;
            _solved = solved;
            _hintsUsed = hintsUsed;
            _scoreEarned = scoreEarned;
            _startedAt = startedAt;
            _solvedAt = solvedAt;
            _timeToSolve = timeToSolve;
            _isDeleted = isDeleted;
            _deletedAt = deletedAt;
        }
        public ASessionProgress() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class AUserStatistic : INotifyPropertyChanged
    {
        private int _userId;
        private string _userLogin = string.Empty;
        private string _username = string.Empty;
        private string _email = string.Empty;
        private int _totalSessions;
        private int _totalPuzzlesSolved;
        private int _totalScore;
        private int _totalHintsUsed;
        private decimal _avgScorePerSession;
        private DateTime? _lastActive;
        private DateTime? _registeredAt;

        public int UserId { get => _userId; set { _userId = value; OnPropertyChanged(); } }
        public string UserLogin { get => _userLogin; set { _userLogin = value; OnPropertyChanged(); } }
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        public int TotalSessions { get => _totalSessions; set { _totalSessions = value; OnPropertyChanged(); } }
        public int TotalPuzzlesSolved { get => _totalPuzzlesSolved; set { _totalPuzzlesSolved = value; OnPropertyChanged(); } }
        public int TotalScore { get => _totalScore; set { _totalScore = value; OnPropertyChanged(); } }
        public int TotalHintsUsed { get => _totalHintsUsed; set { _totalHintsUsed = value; OnPropertyChanged(); } }
        public decimal AvgScorePerSession { get => _avgScorePerSession; set { _avgScorePerSession = value; OnPropertyChanged(); } }
        public DateTime? LastActive { get => _lastActive; set { _lastActive = value; OnPropertyChanged(); } }
        public DateTime? RegisteredAt { get => _registeredAt; set { _registeredAt = value; OnPropertyChanged(); } }

        public double SuccessRate => TotalPuzzlesSolved > 0
            ? (double)TotalPuzzlesSolved / (TotalPuzzlesSolved + TotalHintsUsed) * 100
            : 0;
        public string LastActiveFormatted => LastActive?.ToString("dd.MM.yyyy HH:mm") ?? "—";

        public AUserStatistic(int userId, string userLogin, string username, string email,
            int totalSessions, int totalPuzzlesSolved, int totalScore, int totalHintsUsed,
            decimal avgScorePerSession, DateTime? lastActive, DateTime? registeredAt = null)
        {
            _userId = userId;
            _userLogin = userLogin;
            _username = username;
            _email = email;
            _totalSessions = totalSessions;
            _totalPuzzlesSolved = totalPuzzlesSolved;
            _totalScore = totalScore;
            _totalHintsUsed = totalHintsUsed;
            _avgScorePerSession = avgScorePerSession;
            _lastActive = lastActive;
            _registeredAt = registeredAt;
        }
        public AUserStatistic() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

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

    public record AGameSessionCreate(int UserId, string SessionType, int TotalScore);
    public record AGameSessionUpdate(int Id, int? TotalScore, bool? IsCompleted, DateTime? CompletedAt, int? CurrentTutorialIndex);

    public record ATutorialCreate(int MethodId, string TheoryTitle, string TheoryContent, int SortOrder);
    public record ATutorialUpdate(int Id, int MethodId, string TheoryTitle, string TheoryContent, int SortOrder);

    public record AUserCreate(string Login, string Password, string Username, string Email);
    public record AUserUpdate(int Id, string Login, string Username, string Email, string? Password = null);

    public record ASessionProgressCreate(int SessionId, int PuzzleId, int PuzzleOrder, int HintsUsed, int ScoreEarned);
    public record ASessionProgressUpdate(int Id, int? HintsUsed, int? ScoreEarned, bool? Solved, DateTime? SolvedAt);

    public record AUserStatisticRefresh(int UserId);

    public record UAErrorResponse(string Message, string? Details);
    public record UALoginResponse(int Id, string Login, string Email, string Username, bool IsAdmin);
    public record UALoginRequest(string Login, string Password);
    public record UARegisterRequest(string Login, string Username, string Email, string Password);
    public record UARegisterResponse(int Id, string Login, string Username, string Email);
}