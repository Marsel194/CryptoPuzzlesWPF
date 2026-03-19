using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;
using System.Diagnostics;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class UserProfileViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly UserApiService _userApi;
        private readonly GameSessionApiService _gameSessionApi;
        private readonly SessionProgressApiService _progressApi;
        private readonly UserStatisticsApiService _statisticsApi;
        private readonly PuzzleApiService _puzzleApi;
        private readonly UserSessionService _userSession;
        private readonly Action _closeAction;
        private readonly int _userId;

        private readonly AsyncRelayCommand _saveCommand;

        private string _login = string.Empty;
        private string _email = string.Empty;
        private string _username = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;
        private bool _isEditMode;
        private bool _isLoading;
        private bool _hasActiveSession;

        private string _originalEmail = string.Empty;
        private string _originalUsername = string.Empty;

        private int _trainingProgress;
        private int _practiceProgress;

        public string Login
        {
            get => _login;
            private set => SetProperty(ref _login, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (SetProperty(ref _newPassword, value))
                    AsyncRelayCommand.RaiseCanExecuteChanged();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (SetProperty(ref _confirmPassword, value))
                    AsyncRelayCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool HasActiveSession
        {
            get => _hasActiveSession;
            set => SetProperty(ref _hasActiveSession, value);
        }

        public int TrainingProgress
        {
            get => _trainingProgress;
            private set => SetProperty(ref _trainingProgress, value);
        }

        public int PracticeProgress
        {
            get => _practiceProgress;
            private set => SetProperty(ref _practiceProgress, value);
        }

        public ICommand CloseCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteProgressCommand { get; }

        public UserProfileViewModel(
            IAuthService authService,
            UserApiService userApi,
            GameSessionApiService gameSessionApi,
            SessionProgressApiService progressApi,
            UserStatisticsApiService statisticsApi,
            PuzzleApiService puzzleApi,
            UserSessionService userSession,
            Action closeAction,
            int userId)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _userApi = userApi ?? throw new ArgumentNullException(nameof(userApi));
            _gameSessionApi = gameSessionApi ?? throw new ArgumentNullException(nameof(gameSessionApi));
            _progressApi = progressApi ?? throw new ArgumentNullException(nameof(progressApi));
            _statisticsApi = statisticsApi ?? throw new ArgumentNullException(nameof(statisticsApi));
            _puzzleApi = puzzleApi ?? throw new ArgumentNullException(nameof(puzzleApi));
            _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
            _closeAction = closeAction ?? throw new ArgumentNullException(nameof(closeAction));
            _userId = userId;

            CloseCommand = new AsyncRelayCommand(CloseAsync);
            EditCommand = new AsyncRelayCommand(EditAsync, _ => !IsLoading && !IsEditMode);
            _saveCommand = new AsyncRelayCommand(SaveAsync, _ => !IsLoading && IsEditMode && CanSave());
            SaveCommand = _saveCommand;
            CancelCommand = new AsyncRelayCommand(CancelAsync, _ => !IsLoading && IsEditMode);
            DeleteProgressCommand = new AsyncRelayCommand(DeleteProgressAsync, _ => !IsLoading && HasActiveSession);
        }

        private bool CanSave()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Username))
                return false;

            bool changePassword = !string.IsNullOrWhiteSpace(NewPassword);
            if (changePassword)
            {
                if (NewPassword != ConfirmPassword)
                    return false;
                if (NewPassword.Length < 6)
                    return false;
            }

            return true;
        }

        public async Task LoadUserDataAsync()
        {
            try
            {
                IsLoading = true;

                var user = await _userApi.GetByIdAsync(_userId);
                if (user != null)
                {
                    Login = user.Login;
                    Email = user.Email;
                    Username = user.Username;

                    _originalEmail = user.Email;
                    _originalUsername = user.Username;
                }

                var stats = await _statisticsApi.GetByUserIdAsync(_userId);
                if (stats != null)
                {
                    var allPuzzles = await _puzzleApi.GetAllAsync();
                    var trainingPuzzles = allPuzzles.Where(p => p.IsTraining).ToList();
                    var practicePuzzles = allPuzzles.Where(p => !p.IsTraining).ToList();

                    TrainingProgress = trainingPuzzles.Count > 0
                        ? (int)((double)stats.TotalPuzzlesSolved / trainingPuzzles.Count * 100)
                        : 0;

                    PracticeProgress = practicePuzzles.Count > 0
                        ? (int)((double)stats.TotalPuzzlesSolved / practicePuzzles.Count * 100)
                        : 0;
                }

                var activeSessions = await _gameSessionApi.GetAllAsync(userId: _userId, isCompleted: false);
                HasActiveSession = activeSessions != null && activeSessions.Count != 0;
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка загрузки данных: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteProgressAsync(object? parameter = null)
        {
            try
            {
                var allSessions = await _gameSessionApi.GetByUserIdAsync(_userId);
                if (allSessions == null || allSessions.Count == 0)
                {
                    HasActiveSession = false;
                    return;
                }

                bool confirmed = await DialogService.ShowConfirmation(
                    "Вы действительно хотите удалить ВЕСЬ прогресс? Все ваши сессии будут безвозвратно удалены.");
                if (!confirmed)
                    return;

                IsLoading = true;

                foreach (var session in allSessions)
                {
                    var update = new AGameSessionUpdate(
                        Id: session.Id,
                        TotalScore: null,
                        IsCompleted: null,
                        CompletedAt: null,
                        CurrentTutorialIndex: null,
                        IsDeleted: true,
                        DeletedAt: null
                    );
                    await _gameSessionApi.UpdateAsync(session.Id, update);
                }

                HasActiveSession = false;
                await LoadUserDataAsync();
                await DialogService.ShowMessage("Весь прогресс успешно удалён.");
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка при удалении прогресса: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private Task CloseAsync(object? parameter = null)
        {
            _closeAction?.Invoke();
            return Task.CompletedTask;
        }

        private Task EditAsync(object? parameter = null)
        {
            _originalEmail = Email;
            _originalUsername = Username;

            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;

            IsEditMode = true;
            return Task.CompletedTask;
        }

        private async Task SaveAsync(object? parameter = null)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await DialogService.ShowError("Email не может быть пустым.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                await DialogService.ShowError("Имя не может быть пустым.");
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
                    IsDeleted: null,
                    DeletedAt: null,
                    Password: changePassword ? this.NewPassword : null
                );

                await _userApi.UpdateAsync(_userId, updateDto);

                _originalEmail = Email;
                _originalUsername = Username;

                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;

                IsEditMode = false;

                await DialogService.ShowMessage("Данные успешно сохранены.");
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

        private async Task CancelAsync(object? parameter = null)
        {
            Email = _originalEmail;
            Username = _originalUsername;

            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;

            IsEditMode = false;

            await LoadUserDataAsync();
        }
    }
}