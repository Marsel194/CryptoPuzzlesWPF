using CryptoPuzzles.Converters;
using CryptoPuzzles.Helpers;
using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class UserProfileViewModel : ViewModelBase
    {
        private readonly UserApiService _userApi;
        private readonly PuzzleApiService _puzzleApi;
        private readonly SessionProgressApiService _sessionProgressApi;
        private readonly Action _goBack;
        private readonly int _userId;
        private readonly AsyncRelayCommand _saveCommand;

        private string _login = string.Empty;
        private string _email = string.Empty;
        private string _username = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;
        private int _trainingProgress;
        private int _practiceProgress;
        private bool _isEditMode;

        public UserProfileViewModel(
            UserApiService userApi,
            PuzzleApiService puzzleApi,
            SessionProgressApiService sessionProgressApi,
            int userId,
            Action goBack)
        {
            _userApi = userApi;
            _puzzleApi = puzzleApi;
            _sessionProgressApi = sessionProgressApi;
            _userId = userId;
            _goBack = goBack;

            CloseCommand = new AsyncRelayCommand(CloseAsync);
            EditCommand = new AsyncRelayCommand(EditAsync);
            CancelCommand = new AsyncRelayCommand(CancelAsync);
            _saveCommand = new AsyncRelayCommand(SaveAsync, _ => CanSave());
            SaveCommand = _saveCommand;
        }

        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
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
                    _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (SetProperty(ref _confirmPassword, value))
                    _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public int TrainingProgress
        {
            get => _trainingProgress;
            set => SetProperty(ref _trainingProgress, value);
        }

        public int PracticeProgress
        {
            get => _practiceProgress;
            set => SetProperty(ref _practiceProgress, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public ICommand CloseCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public async Task LoadDataAsync()
        {
            try
            {
                var user = await _userApi.GetByIdAsync(_userId);
                if (user != null)
                {
                    Login = user.Login;
                    Email = user.Email;
                    Username = user.Username;
                }

                var allPuzzles = await _puzzleApi.GetAllAsync();
                int totalTraining = allPuzzles.Count(p => p.IsTraining);
                int totalPractice = allPuzzles.Count(p => !p.IsTraining);

                var solvedProgress = await _sessionProgressApi.GetAllAsync(userId: _userId, solved: true);
                int solvedTraining = solvedProgress.Count(sp =>
                {
                    var puzzle = allPuzzles.FirstOrDefault(p => p.Id == sp.PuzzleId);
                    return puzzle != null && puzzle.IsTraining;
                });
                int solvedPractice = solvedProgress.Count(sp =>
                {
                    var puzzle = allPuzzles.FirstOrDefault(p => p.Id == sp.PuzzleId);
                    return puzzle != null && !puzzle.IsTraining;
                });

                TrainingProgress = totalTraining > 0 ? (solvedTraining * 100 / totalTraining) : 0;
                PracticeProgress = totalPractice > 0 ? (solvedPractice * 100 / totalPractice) : 0;
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка загрузки профиля: {ex.Message}");
            }
        }

        private Task CloseAsync(object? parameter = null)
        {
            _goBack?.Invoke();
            return Task.CompletedTask;
        }

        private Task EditAsync(object? parameter = null)
        {
            IsEditMode = true;
            return Task.CompletedTask;
        }

        private Task CancelAsync(object? parameter = null)
        {
            IsEditMode = false;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            _ = LoadDataAsync();
            return Task.CompletedTask;
        }

        private bool CanSave(object? parameter = null)
        {
            if (!IsEditMode) return false;
            if (string.IsNullOrWhiteSpace(NewPassword) && string.IsNullOrWhiteSpace(ConfirmPassword))
                return true;
            return NewPassword == ConfirmPassword && !string.IsNullOrWhiteSpace(NewPassword);
        }

        private async Task SaveAsync(object? parameter = null)
        {
            try
            {
                var updateDto = new AUserUpdate(
                    Id: _userId,
                    Login: Login,
                    Username: Username,
                    Email: Email,
                    Password: string.IsNullOrWhiteSpace(NewPassword) ? null : NewPassword
                );

                await _userApi.UpdateAsync(_userId, updateDto);

                await DialogService.ShowMessage("Данные успешно сохранены");
                IsEditMode = false;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}