using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class UserProfileViewModel : ViewModelBase
    {
        private readonly UserApiService _userApiService;
        private readonly GameSessionApiService _sessionApiService;
        private readonly PuzzleApiService _puzzleApiService;
        private readonly Action _closeAction;
        private readonly int _userId;

        private string _username = string.Empty;
        private string _login = string.Empty;
        private string _email = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;
        private int _trainingProgress;
        private int _practiceProgress;
        private bool _isEditMode;
        private bool _isLoading;

        private string _originalUsername = string.Empty;
        private string _originalEmail = string.Empty;
        private int _totalTrainingPuzzles;
        private int _totalPracticePuzzles;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string NewPassword
        {
            get => _newPassword;
            set { _newPassword = value; OnPropertyChanged(); }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        public int TrainingProgress
        {
            get => _trainingProgress;
            set { _trainingProgress = value; OnPropertyChanged(); }
        }

        public int PracticeProgress
        {
            get => _practiceProgress;
            set { _practiceProgress = value; OnPropertyChanged(); }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                if (_isEditMode != value)
                {
                    _isEditMode = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public ICommand CloseCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public UserProfileViewModel(
            UserApiService userApiService,
            GameSessionApiService sessionApiService,
            PuzzleApiService puzzleApiService,
            AUser user,
            Action closeAction)
        {
            _userApiService = userApiService;
            _sessionApiService = sessionApiService;
            _puzzleApiService = puzzleApiService;
            _closeAction = closeAction;
            _userId = user.Id;

            Username = user.Username;
            Login = user.Login;
            Email = user.Email;

            _originalUsername = user.Username;
            _originalEmail = user.Email;

            CloseCommand = new AsyncRelayCommand(CloseAsync);
            EditCommand = new AsyncRelayCommand(EditAsync, () => !IsLoading && !IsEditMode);
            SaveCommand = new AsyncRelayCommand(SaveAsync, () => !IsLoading && IsEditMode);
            CancelCommand = new AsyncRelayCommand(CancelAsync, () => !IsLoading && IsEditMode);

            _ = LoadProgressAsync();
        }

        private Task CloseAsync()
        {
            _closeAction?.Invoke();
            return Task.CompletedTask;
        }

        private Task EditAsync()
        {
            _originalUsername = Username;
            _originalEmail = Email;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            IsEditMode = true;
            return Task.CompletedTask;
        }

        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
            {
                await DialogService.ShowError("Введите корректный email, содержащий '@'.");
                return;
            }

            bool changePassword = !string.IsNullOrWhiteSpace(NewPassword);
            if (changePassword)
            {
                if (NewPassword != ConfirmPassword)
                {
                    await DialogService.ShowError("Введенные пароли не совпадают.");
                    return;
                }
                if (NewPassword.Length < 6)
                {
                    await DialogService.ShowError("Пароль должен содержать не менее 6 символов.");
                    return;
                }
            }

            try
            {
                IsLoading = true;

                var updateDto = new AUserUpdate(
                    Id: _userId,
                    Login: this.Login,
                    Username: this.Username,
                    Email: this.Email,
                    Password: changePassword ? this.NewPassword : null
                );

                await _userApiService.UpdateAsync(_userId, updateDto);

                _originalUsername = Username;
                _originalEmail = Email;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;
                IsEditMode = false;

                await DialogService.ShowMessage("Данные успешно сохранены.");
                await LoadProgressAsync();
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка при сохранении: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private Task CancelAsync()
        {
            Username = _originalUsername;
            Email = _originalEmail;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            IsEditMode = false;
            return Task.CompletedTask;
        }

        private async Task LoadProgressAsync()
        {
            try
            {
                IsLoading = true;

                var allPuzzles = await _puzzleApiService.GetAllAsync();
                var activePuzzles = allPuzzles.Where(p => !p.IsDeleted).ToList();

                _totalTrainingPuzzles = activePuzzles.Count(p => p.IsTraining);
                _totalPracticePuzzles = activePuzzles.Count(p => !p.IsTraining);

                var allSessions = await _sessionApiService.GetAllAsync();
                var completedSessions = allSessions
                    .Where(s => s.UserId == _userId && s.CompletedAt != null && !s.IsDeleted)
                    .ToList();

                var puzzlesDict = activePuzzles.ToDictionary(p => p.Id, p => p);
                var completedTrainingIds = new HashSet<int>();
                var completedPracticeIds = new HashSet<int>();

                foreach (var session in completedSessions)
                {
                    if (session.CurrentPuzzleId.HasValue &&
                        puzzlesDict.TryGetValue(session.CurrentPuzzleId.Value, out var puzzle))
                    {
                        if (puzzle.IsTraining)
                            completedTrainingIds.Add(puzzle.Id);
                        else
                            completedPracticeIds.Add(puzzle.Id);
                    }
                }

                TrainingProgress = _totalTrainingPuzzles > 0
                    ? (int)((double)completedTrainingIds.Count / _totalTrainingPuzzles * 100) : 0;
                PracticeProgress = _totalPracticePuzzles > 0
                    ? (int)((double)completedPracticeIds.Count / _totalPracticePuzzles * 100) : 0;

                TrainingProgress = Math.Min(100, Math.Max(0, TrainingProgress));
                PracticeProgress = Math.Min(100, Math.Max(0, PracticeProgress));
            }
            catch
            {
                TrainingProgress = 0;
                PracticeProgress = 0;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}