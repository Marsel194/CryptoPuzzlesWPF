using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CryptoPuzzles.Shared
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    // Интерфейсы
    public interface IHasId { int Id { get; } }
    public interface IEntityWithId { int Id { get; set; } }
    public interface ISoftDelete { bool IsDeleted { get; set; } DateTime? DeletedAt { get; set; } }
    public interface IHasCreatedAt { DateTime CreatedAt { get; set; } }

    // Классы сущностей
    public class AAdmin : ObservableObject
    {
        private int _id; private string _login = string.Empty; private string _firstName = string.Empty;
        private string _lastName = string.Empty; private string? _middleName; private DateTime _createdAt;
        private bool _isDeleted; private DateTime? _deletedAt;
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        [ExportName("Логин")] public string Login { get => _login; set => SetProperty(ref _login, value); }
        [ExportName("Имя")] public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
        [ExportName("Фамилия")] public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
        [ExportName("Отчество")] public string? MiddleName { get => _middleName; set => SetProperty(ref _middleName, value); }
        [ExportName("Дата создания")] public DateTime CreatedAt { get => _createdAt; set => SetProperty(ref _createdAt, value); }
        public bool IsDeleted { get => _isDeleted; set => SetProperty(ref _isDeleted, value); }
        public DateTime? DeletedAt { get => _deletedAt; set => SetProperty(ref _deletedAt, value); }
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
        public AAdmin(int id, string login, string firstName, string lastName, string? middleName, DateTime createdAt, bool isDeleted = false, DateTime? deletedAt = null)
        { _id = id; _login = login; _firstName = firstName; _lastName = lastName; _middleName = middleName; _createdAt = createdAt; _isDeleted = isDeleted; _deletedAt = deletedAt; }
        public AAdmin() { }
    }

    public class ADifficulty : ObservableObject
    {
        private int _id; private string _difficultyName = string.Empty; private bool _isDeleted; private DateTime? _deletedAt;
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        [ExportName("Название сложности")] public string DifficultyName { get => _difficultyName; set => SetProperty(ref _difficultyName, value); }
        public bool IsDeleted { get => _isDeleted; set => SetProperty(ref _isDeleted, value); }
        public DateTime? DeletedAt { get => _deletedAt; set => SetProperty(ref _deletedAt, value); }
        public ADifficulty(int id, string difficultyName, bool isDeleted = false, DateTime? deletedAt = null)
        { _id = id; _difficultyName = difficultyName; _isDeleted = isDeleted; _deletedAt = deletedAt; }
        public ADifficulty() { }
    }

    public class AEncryptionMethod : ObservableObject
    {
        private int _id; private string _name = string.Empty; private bool _isDeleted; private DateTime? _deletedAt;
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        [ExportName("Название метода")] public string Name { get => _name; set => SetProperty(ref _name, value); }
        public bool IsDeleted { get => _isDeleted; set => SetProperty(ref _isDeleted, value); }
        public DateTime? DeletedAt { get => _deletedAt; set => SetProperty(ref _deletedAt, value); }
        public AEncryptionMethod(int id, string name, bool isDeleted = false, DateTime? deletedAt = null)
        { _id = id; _name = name; _isDeleted = isDeleted; _deletedAt = deletedAt; }
        public AEncryptionMethod() { }
    }

    public class APuzzle : ObservableObject
    {
        private int _id; private string _title = string.Empty; private string _content = string.Empty; private string _answer = string.Empty;
        private int _maxScore; private int _difficultyId; private string _difficultyName = string.Empty; private int? _methodId;
        private string? _methodName; private bool _isTraining; private int? _tutorialOrder; private int? _createdByAdminId;
        private string? _createdByAdminName; private DateTime _createdAt; private bool _isDeleted; private DateTime? _deletedAt;
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        [ExportName("Название")] public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string Content { get => _content; set => SetProperty(ref _content, value); }
        public string Answer { get => _answer; set => SetProperty(ref _answer, value); }
        [ExportName("Макс. балл")] public int MaxScore { get => _maxScore; set => SetProperty(ref _maxScore, value); }
        public int DifficultyId { get => _difficultyId; set => SetProperty(ref _difficultyId, value); }
        [ExportName("Сложность")] public string DifficultyName { get => _difficultyName; set => SetProperty(ref _difficultyName, value); }
        public int? MethodId { get => _methodId; set => SetProperty(ref _methodId, value); }
        [ExportName("Метод")] public string? MethodName { get => _methodName; set => SetProperty(ref _methodName, value); }
        public bool IsTraining { get => _isTraining; set => SetProperty(ref _isTraining, value); }
        [ExportName("Порядок обучения")] public int? TutorialOrder { get => _tutorialOrder; set => SetProperty(ref _tutorialOrder, value); }
        public int? CreatedByAdminId { get => _createdByAdminId; set => SetProperty(ref _createdByAdminId, value); }
        [ExportName("Создал администратор")] public string? CreatedByAdminName { get => _createdByAdminName; set => SetProperty(ref _createdByAdminName, value); }
        [ExportName("Дата создания")] public DateTime CreatedAt { get => _createdAt; set => SetProperty(ref _createdAt, value); }
        public bool IsDeleted { get => _isDeleted; set => SetProperty(ref _isDeleted, value); }
        public DateTime? DeletedAt { get => _deletedAt; set => SetProperty(ref _deletedAt, value); }
        [ExportName("Тип")] public string PuzzleType => IsTraining ? "Обучение" : "Практика";
        public string HasMethod => MethodId.HasValue ? "Да" : "Нет";
        public APuzzle(int id, string title, string content, string answer, int maxScore, int difficultyId, string difficultyName, int? methodId, string? methodName, bool isTraining, int? tutorialOrder, int? createdByAdminId, string? createdByAdminName, DateTime createdAt, bool isDeleted = false, DateTime? deletedAt = null)
        { _id = id; _title = title; _content = content; _answer = answer; _maxScore = maxScore; _difficultyId = difficultyId; _difficultyName = difficultyName; _methodId = methodId; _methodName = methodName; _isTraining = isTraining; _tutorialOrder = tutorialOrder; _createdByAdminId = createdByAdminId; _createdByAdminName = createdByAdminName; _createdAt = createdAt; _isDeleted = isDeleted; _deletedAt = deletedAt; }
        public APuzzle() { }
    }

    public class AHint : ObservableObject
    {
        private int _id; private int _puzzleId; private string _puzzleTitle = string.Empty; private string _hintText = string.Empty;
        private int _hintOrder; private DateTime _createdAt; private bool _isDeleted; private DateTime? _deletedAt;
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        public int PuzzleId { get => _puzzleId; set => SetProperty(ref _puzzleId, value); }
        public string PuzzleTitle { get => _puzzleTitle; set => SetProperty(ref _puzzleTitle, value); }
        public string HintText { get => _hintText; set => SetProperty(ref _hintText, value); }
        public int HintOrder { get => _hintOrder; set => SetProperty(ref _hintOrder, value); }
        public DateTime CreatedAt { get => _createdAt; set => SetProperty(ref _createdAt, value); }
        public bool IsDeleted { get => _isDeleted; set => SetProperty(ref _isDeleted, value); }
        public DateTime? DeletedAt { get => _deletedAt; set => SetProperty(ref _deletedAt, value); }
        public AHint(int id, int puzzleId, string puzzleTitle, string hintText, int hintOrder, DateTime createdAt, bool isDeleted = false, DateTime? deletedAt = null)
        { _id = id; _puzzleId = puzzleId; _puzzleTitle = puzzleTitle; _hintText = hintText; _hintOrder = hintOrder; _createdAt = createdAt; _isDeleted = isDeleted; _deletedAt = deletedAt; }
        public AHint() { }
    }

    public class AGameSession : ObservableObject
    {
        private int _id; private int _userId; private string _userLogin = string.Empty; private string _username = string.Empty;
        private string _sessionType = string.Empty; private int _totalScore; private DateTime _sessionStart; private DateTime? _completedAt;
        private bool _isCompleted; private bool _isDeleted; private DateTime? _deletedAt; private int _puzzlesCount; private int _solvedCount;
        private int? _currentTutorialIndex;
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        public int UserId { get => _userId; set => SetProperty(ref _userId, value); }
        [ExportName("Логин пользователя")] public string UserLogin { get => _userLogin; set => SetProperty(ref _userLogin, value); }
        [ExportName("Имя пользователя")] public string Username { get => _username; set => SetProperty(ref _username, value); }
        [ExportName("Тип сессии")] public string SessionType { get => _sessionType; set => SetProperty(ref _sessionType, value); }
        [ExportName("Всего очков")] public int TotalScore { get => _totalScore; set => SetProperty(ref _totalScore, value); }
        [ExportName("Начало сессии")] public DateTime SessionStart { get => _sessionStart; set => SetProperty(ref _sessionStart, value); }
        [ExportName("Завершена")] public DateTime? CompletedAt { get => _completedAt; set => SetProperty(ref _completedAt, value); }
        public bool IsCompleted { get => _isCompleted; set => SetProperty(ref _isCompleted, value); }
        public bool IsDeleted { get => _isDeleted; set => SetProperty(ref _isDeleted, value); }
        public DateTime? DeletedAt { get => _deletedAt; set => SetProperty(ref _deletedAt, value); }
        [ExportName("Всего головоломок")] public int PuzzlesCount { get => _puzzlesCount; set => SetProperty(ref _puzzlesCount, value); }
        [ExportName("Решено")] public int SolvedCount { get => _solvedCount; set => SetProperty(ref _solvedCount, value); }
        [ExportName("Текущий шаг")] public int? CurrentTutorialIndex { get => _currentTutorialIndex; set => SetProperty(ref _currentTutorialIndex, value); }
        [ExportName("Статус")] public string Status => IsCompleted ? "Завершена" : "Активна";
        [ExportName("Длительность")] public string Duration => CompletedAt.HasValue ? (CompletedAt.Value - SessionStart).ToString(@"hh\:mm\:ss") : "—";
        [ExportName("Прогресс %")] public double Progress => PuzzlesCount > 0 ? (double)SolvedCount / PuzzlesCount * 100 : 0;
        public AGameSession(int id, int userId, string userLogin, string username, string sessionType, int totalScore, DateTime sessionStart, DateTime? completedAt, bool isCompleted, int puzzlesCount, int solvedCount, int? currentTutorialIndex, bool isDeleted = false, DateTime? deletedAt = null)
        { _id = id; _userId = userId; _userLogin = userLogin; _username = username; _sessionType = sessionType; _totalScore = totalScore; _sessionStart = sessionStart; _completedAt = completedAt; _isCompleted = isCompleted; _puzzlesCount = puzzlesCount; _solvedCount = solvedCount; _currentTutorialIndex = currentTutorialIndex; _isDeleted = isDeleted; _deletedAt = deletedAt; }
        public AGameSession() { }
    }

    public class ATutorial : ObservableObject
    {
        private int _id; private int _methodId; private string _methodName = string.Empty; private string _theoryTitle = string.Empty;
        private string _theoryContent = string.Empty; private int _sortOrder; private DateTime _createdAt; private bool _isDeleted; private DateTime? _deletedAt;
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        public int MethodId { get => _methodId; set => SetProperty(ref _methodId, value); }
        [ExportName("Метод")] public string MethodName { get => _methodName; set => SetProperty(ref _methodName, value); }
        [ExportName("Название теории")] public string TheoryTitle { get => _theoryTitle; set => SetProperty(ref _theoryTitle, value); }
        public string TheoryContent { get => _theoryContent; set => SetProperty(ref _theoryContent, value); }
        [ExportName("Порядок")] public int SortOrder { get => _sortOrder; set => SetProperty(ref _sortOrder, value); }
        [ExportName("Дата создания")] public DateTime CreatedAt { get => _createdAt; set => SetProperty(ref _createdAt, value); }
        public bool IsDeleted { get => _isDeleted; set => SetProperty(ref _isDeleted, value); }
        public DateTime? DeletedAt { get => _deletedAt; set => SetProperty(ref _deletedAt, value); }
        public ATutorial(int id, int methodId, string methodName, string theoryTitle, string theoryContent, int sortOrder, DateTime createdAt, bool isDeleted = false, DateTime? deletedAt = null)
        { _id = id; _methodId = methodId; _methodName = methodName; _theoryTitle = theoryTitle; _theoryContent = theoryContent; _sortOrder = sortOrder; _createdAt = createdAt; _isDeleted = isDeleted; _deletedAt = deletedAt; }
        public ATutorial() { }
    }

    public class AUser : ObservableObject
    {
        private int _id; private string _login = string.Empty; private string _username = string.Empty; private string _email = string.Empty;
        private DateTime? _createdAt; private bool _isDeleted; private DateTime? _deletedAt; private int _totalSessions; private int _totalScore; private int _puzzlesSolved;
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        [ExportName("Логин")] public string Login { get => _login; set => SetProperty(ref _login, value); }
        [ExportName("Имя пользователя")] public string Username { get => _username; set => SetProperty(ref _username, value); }
        [ExportName("Почта")] public string Email { get => _email; set => SetProperty(ref _email, value); }
        [ExportName("Дата регистрации")] public DateTime? CreatedAt { get => _createdAt; set => SetProperty(ref _createdAt, value); }
        public bool IsDeleted { get => _isDeleted; set => SetProperty(ref _isDeleted, value); }
        public DateTime? DeletedAt { get => _deletedAt; set => SetProperty(ref _deletedAt, value); }
        [ExportName("Всего сессий")] public int TotalSessions { get => _totalSessions; set => SetProperty(ref _totalSessions, value); }
        [ExportName("Всего очков")] public int TotalScore { get => _totalScore; set => SetProperty(ref _totalScore, value); }
        [ExportName("Решено головоломок")] public int PuzzlesSolved { get => _puzzlesSolved; set => SetProperty(ref _puzzlesSolved, value); }
        public AUser(int id, string login, string username, string email, DateTime? createdAt, bool isDeleted = false, DateTime? deletedAt = null, int totalSessions = 0, int totalScore = 0, int puzzlesSolved = 0)
        { _id = id; _login = login; _username = username; _email = email; _createdAt = createdAt; _isDeleted = isDeleted; _deletedAt = deletedAt; _totalSessions = totalSessions; _totalScore = totalScore; _puzzlesSolved = puzzlesSolved; }
        public AUser() { }
    }

    public class ASessionProgress : ObservableObject
    {
        private int _id; private int _sessionId; private string _userLogin = string.Empty; private string _username = string.Empty;
        private int _puzzleId; private string _puzzleTitle = string.Empty; private int _puzzleOrder; private bool _solved;
        private int _hintsUsed; private int _scoreEarned; private DateTime _startedAt; private DateTime? _solvedAt;
        private TimeSpan? _timeToSolve; private bool? _isDeleted; private DateTime? _deletedAt;
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        public int SessionId { get => _sessionId; set => SetProperty(ref _sessionId, value); }
        [ExportName("Логин пользователя")] public string UserLogin { get => _userLogin; set => SetProperty(ref _userLogin, value); }
        [ExportName("Имя пользователя")] public string Username { get => _username; set => SetProperty(ref _username, value); }
        public int PuzzleId { get => _puzzleId; set => SetProperty(ref _puzzleId, value); }
        [ExportName("Головоломка")] public string PuzzleTitle { get => _puzzleTitle; set => SetProperty(ref _puzzleTitle, value); }
        [ExportName("Порядок")] public int PuzzleOrder { get => _puzzleOrder; set => SetProperty(ref _puzzleOrder, value); }
        public bool Solved { get => _solved; set => SetProperty(ref _solved, value); }
        [ExportName("Использовано подсказок")] public int HintsUsed { get => _hintsUsed; set => SetProperty(ref _hintsUsed, value); }
        [ExportName("Заработано очков")] public int ScoreEarned { get => _scoreEarned; set => SetProperty(ref _scoreEarned, value); }
        [ExportName("Начало")] public DateTime StartedAt { get => _startedAt; set => SetProperty(ref _startedAt, value); }
        [ExportName("Время решения")] public DateTime? SolvedAt { get => _solvedAt; set => SetProperty(ref _solvedAt, value); }
        [ExportName("Затраченное время")] public TimeSpan? TimeToSolve { get => _timeToSolve; set => SetProperty(ref _timeToSolve, value); }
        public bool? IsDeleted { get => _isDeleted; set => SetProperty(ref _isDeleted, value); }
        public DateTime? DeletedAt { get => _deletedAt; set => SetProperty(ref _deletedAt, value); }
        [ExportName("Статус")] public string Status => Solved ? "Решено" : "Не решено";
        [ExportName("Время")] public string TimeSpent => TimeToSolve?.ToString(@"hh\:mm\:ss") ?? "—";
        public ASessionProgress(int id, int sessionId, string userLogin, string username, int puzzleId, string puzzleTitle, int puzzleOrder, bool solved, int hintsUsed, int scoreEarned, DateTime startedAt, DateTime? solvedAt, TimeSpan? timeToSolve, bool isDeleted = false, DateTime? deletedAt = null)
        { _id = id; _sessionId = sessionId; _userLogin = userLogin; _username = username; _puzzleId = puzzleId; _puzzleTitle = puzzleTitle; _puzzleOrder = puzzleOrder; _solved = solved; _hintsUsed = hintsUsed; _scoreEarned = scoreEarned; _startedAt = startedAt; _solvedAt = solvedAt; _timeToSolve = timeToSolve; _isDeleted = isDeleted; _deletedAt = deletedAt; }
        public ASessionProgress() { }
    }

    public class AUserStatistic : ObservableObject
    {
        private int _userId; private string _userLogin = string.Empty; private string _username = string.Empty; private string _email = string.Empty;
        private int _totalSessions; private int _totalPuzzlesSolved; private int _totalScore; private int _totalHintsUsed;
        private decimal _avgScorePerSession; private DateTime? _lastActive; private DateTime? _registeredAt; private bool _isDeleted; private DateTime? _deletedAt;
        public int UserId { get => _userId; set => SetProperty(ref _userId, value); }
        [ExportName("Логин пользователя")] public string UserLogin { get => _userLogin; set => SetProperty(ref _userLogin, value); }
        [ExportName("Имя пользователя")] public string Username { get => _username; set => SetProperty(ref _username, value); }
        [ExportName("Почта")] public string Email { get => _email; set => SetProperty(ref _email, value); }
        [ExportName("Всего сессий")] public int TotalSessions { get => _totalSessions; set => SetProperty(ref _totalSessions, value); }
        [ExportName("Решено головоломок")] public int TotalPuzzlesSolved { get => _totalPuzzlesSolved; set => SetProperty(ref _totalPuzzlesSolved, value); }
        [ExportName("Всего очков")] public int TotalScore { get => _totalScore; set => SetProperty(ref _totalScore, value); }
        [ExportName("Всего подсказок")] public int TotalHintsUsed { get => _totalHintsUsed; set => SetProperty(ref _totalHintsUsed, value); }
        [ExportName("Средний балл за сессию")] public decimal AvgScorePerSession { get => _avgScorePerSession; set => SetProperty(ref _avgScorePerSession, value); }
        [ExportName("Последняя активность")] public DateTime? LastActive { get => _lastActive; set => SetProperty(ref _lastActive, value); }
        [ExportName("Дата регистрации")] public DateTime? RegisteredAt { get => _registeredAt; set => SetProperty(ref _registeredAt, value); }
        public bool IsDeleted { get => _isDeleted; set => SetProperty(ref _isDeleted, value); }
        public DateTime? DeletedAt { get => _deletedAt; set => SetProperty(ref _deletedAt, value); }
        [ExportName("Процент успеха")] public double SuccessRate => TotalPuzzlesSolved > 0 ? (double)TotalPuzzlesSolved / (TotalPuzzlesSolved + TotalHintsUsed) * 100 : 0;
        [ExportName("Последняя активность")] public string LastActiveFormatted => LastActive?.ToString("dd.MM.yyyy HH:mm") ?? "—";
        public AUserStatistic(int userId, string userLogin, string username, string email, int totalSessions, int totalPuzzlesSolved, int totalScore, int totalHintsUsed, decimal avgScorePerSession, DateTime? lastActive, DateTime? registeredAt = null)
        { _userId = userId; _userLogin = userLogin; _username = username; _email = email; _totalSessions = totalSessions; _totalPuzzlesSolved = totalPuzzlesSolved; _totalScore = totalScore; _totalHintsUsed = totalHintsUsed; _avgScorePerSession = avgScorePerSession; _lastActive = lastActive; _registeredAt = registeredAt; }
        public AUserStatistic() { }
    }

    // Records для операций создания/обновления
    public record AAdminCreate(string Login, string Password, string FirstName, string LastName, string? MiddleName);
    public record AAdminUpdate(int Id, string Login, string FirstName, string LastName, string? MiddleName, string? Password, bool? IsDeleted, DateTime? DeletedAt) : IHasId;
    public record ADifficultyCreate(string DifficultyName);
    public record ADifficultyUpdate(int Id, string DifficultyName, bool? IsDeleted, DateTime? DeletedAt) : IHasId;
    public record AEncryptionMethodCreate(string Name);
    public record AEncryptionMethodUpdate(int Id, string Name, bool? IsDeleted, DateTime? DeletedAt) : IHasId;
    public record APuzzleCreate(string Title, string Content, string Answer, int MaxScore, int DifficultyId, int? MethodId, bool IsTraining, int? TutorialOrder);
    public record APuzzleUpdate(int Id, string Title, string Content, string Answer, int MaxScore, int DifficultyId, int? MethodId, bool IsTraining, int? TutorialOrder, bool? IsDeleted, DateTime? DeletedAt) : IHasId;
    public record AHintCreate(int PuzzleId, string HintText, int HintOrder);
    public record AHintUpdate(int Id, int PuzzleId, string HintText, int HintOrder, bool? IsDeleted, DateTime? DeletedAt) : IHasId;
    public record AGameSessionCreate(int UserId, string SessionType, int TotalScore);
    public record AGameSessionUpdate(int Id, int? TotalScore, bool? IsCompleted, DateTime? CompletedAt, int? CurrentTutorialIndex, bool? IsDeleted, DateTime? DeletedAt) : IHasId;
    public record ATutorialCreate(int MethodId, string TheoryTitle, string TheoryContent, int SortOrder);
    public record ATutorialUpdate(int Id, int MethodId, string TheoryTitle, string TheoryContent, int SortOrder, bool? IsDeleted, DateTime? DeletedAt) : IHasId;
    public record AUserCreate(string Login, string Password, string Username, string Email);
    public record AUserUpdate(int Id, string Login, string Username, string Email, bool? IsDeleted, DateTime? DeletedAt, string? Password = null) : IHasId;
    public record ASessionProgressCreate(int SessionId, int PuzzleId, int PuzzleOrder, int HintsUsed, int ScoreEarned);
    public record ASessionProgressUpdate(int Id, int? HintsUsed, int? ScoreEarned, bool? Solved, DateTime? SolvedAt, bool? IsDeleted, DateTime? DeletedAt) : IHasId;
    public record AUserStatisticRefresh(int UserId);

    // Общие record для ответов/запросов
    public record UAErrorResponse(string Message, string? Details);
    public record UALoginResponse(int Id, string Login, string Email, string Username, bool IsAdmin);
    public record UALoginRequest(string Login, string Password);
    public record UARegisterRequest(string Login, string Username, string Email, string Password);
    public record UARegisterResponse(int Id, string Login, string Username, string Email);
}