using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CryptoPuzzles.Shared
{
    // ---- Изменяемые классы для отображения и редактирования в DataGrid ----

    public class AAdmin : INotifyPropertyChanged
    {
        private int _id;
        private string _login = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string? _middleName;
        private DateTime _createdAt;

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string Login { get => _login; set { _login = value; OnPropertyChanged(); } }
        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(); } }
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(); } }
        public string? MiddleName { get => _middleName; set { _middleName = value; OnPropertyChanged(); } }
        public DateTime CreatedAt { get => _createdAt; set { _createdAt = value; OnPropertyChanged(); } }

        public AAdmin(int id, string login, string firstName, string lastName, string? middleName, DateTime createdAt)
        {
            _id = id;
            _login = login;
            _firstName = firstName;
            _lastName = lastName;
            _middleName = middleName;
            _createdAt = createdAt;
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

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string DifficultyName { get => _difficultyName; set { _difficultyName = value; OnPropertyChanged(); } }

        public ADifficulty(int id, string difficultyName)
        {
            _id = id;
            _difficultyName = difficultyName;
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

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        public AEncryptionMethod(int id, string name)
        {
            _id = id;
            _name = name;
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
        private DateTime _createdAt;

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
        public DateTime CreatedAt { get => _createdAt; set { _createdAt = value; OnPropertyChanged(); } }

        public APuzzle(int id, string title, string content, string answer, int maxScore,
            int difficultyId, string difficultyName, int? methodId, string? methodName,
            bool isTraining, int? tutorialOrder, int? createdByAdminId, DateTime createdAt)
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
            _createdAt = createdAt;
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

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public int PuzzleId { get => _puzzleId; set { _puzzleId = value; OnPropertyChanged(); } }
        public string PuzzleTitle { get => _puzzleTitle; set { _puzzleTitle = value; OnPropertyChanged(); } }
        public string HintText { get => _hintText; set { _hintText = value; OnPropertyChanged(); } }
        public int HintOrder { get => _hintOrder; set { _hintOrder = value; OnPropertyChanged(); } }

        public AHint(int id, int puzzleId, string puzzleTitle, string hintText, int hintOrder)
        {
            _id = id;
            _puzzleId = puzzleId;
            _puzzleTitle = puzzleTitle;
            _hintText = hintText;
            _hintOrder = hintOrder;
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
        private int _score;
        private DateTime _sessionStartTime;
        private int? _currentPuzzleId;
        private string? _currentPuzzleTitle;
        private bool _trainingCompleted;
        private int _hintsUsed;
        private DateTime? _completedAt;

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public int UserId { get => _userId; set { _userId = value; OnPropertyChanged(); } }
        public string UserLogin { get => _userLogin; set { _userLogin = value; OnPropertyChanged(); } }
        public int Score { get => _score; set { _score = value; OnPropertyChanged(); } }
        public DateTime SessionStartTime { get => _sessionStartTime; set { _sessionStartTime = value; OnPropertyChanged(); } }
        public int? CurrentPuzzleId { get => _currentPuzzleId; set { _currentPuzzleId = value; OnPropertyChanged(); } }
        public string? CurrentPuzzleTitle { get => _currentPuzzleTitle; set { _currentPuzzleTitle = value; OnPropertyChanged(); } }
        public bool TrainingCompleted { get => _trainingCompleted; set { _trainingCompleted = value; OnPropertyChanged(); } }
        public int HintsUsed { get => _hintsUsed; set { _hintsUsed = value; OnPropertyChanged(); } }
        public DateTime? CompletedAt { get => _completedAt; set { _completedAt = value; OnPropertyChanged(); } }

        public AGameSession(int id, int userId, string userLogin, int score, DateTime sessionStartTime,
            int? currentPuzzleId, string? currentPuzzleTitle, bool trainingCompleted, int hintsUsed, DateTime? completedAt)
        {
            _id = id;
            _userId = userId;
            _userLogin = userLogin;
            _score = score;
            _sessionStartTime = sessionStartTime;
            _currentPuzzleId = currentPuzzleId;
            _currentPuzzleTitle = currentPuzzleTitle;
            _trainingCompleted = trainingCompleted;
            _hintsUsed = hintsUsed;
            _completedAt = completedAt;
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

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public int MethodId { get => _methodId; set { _methodId = value; OnPropertyChanged(); } }
        public string MethodName { get => _methodName; set { _methodName = value; OnPropertyChanged(); } }
        public string TheoryTitle { get => _theoryTitle; set { _theoryTitle = value; OnPropertyChanged(); } }
        public string TheoryContent { get => _theoryContent; set { _theoryContent = value; OnPropertyChanged(); } }
        public int SortOrder { get => _sortOrder; set { _sortOrder = value; OnPropertyChanged(); } }
        public DateTime CreatedAt { get => _createdAt; set { _createdAt = value; OnPropertyChanged(); } }

        public ATutorial(int id, int methodId, string methodName, string theoryTitle, string theoryContent, int sortOrder, DateTime createdAt)
        {
            _id = id;
            _methodId = methodId;
            _methodName = methodName;
            _theoryTitle = theoryTitle;
            _theoryContent = theoryContent;
            _sortOrder = sortOrder;
            _createdAt = createdAt;
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

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string Login { get => _login; set { _login = value; OnPropertyChanged(); } }
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        public DateTime? CreatedAt { get => _createdAt; set { _createdAt = value; OnPropertyChanged(); } }

        public AUser(int id, string login, string username, string email, DateTime? createdAt)
        {
            _id = id;
            _login = login;
            _username = username;
            _email = email;
            _createdAt = createdAt;
        }
        public AUser() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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