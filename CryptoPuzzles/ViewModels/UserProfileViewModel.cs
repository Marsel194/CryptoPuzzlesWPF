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

        // Данные пользователя
        private string _login = string.Empty;
        private string _email = string.Empty;
        private string _username = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;

        // Прогресс
        private int _trainingProgress;
        private int _practiceProgress;

        // Режим редактирования
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

            CloseCommand = new AsyncRelayCommand(async _ => { _goBack(); await Task.CompletedTask; });
            EditCommand = new AsyncRelayCommand(_ => { IsEditMode = true; return Task.CompletedTask; });
            CancelCommand = new AsyncRelayCommand(_ => { CancelEdit(); return Task.CompletedTask; });
            SaveCommand = new AsyncRelayCommand(async _ => await SaveAsync(), _ => CanSave());

            LoadUserDataAsync().SafeFireAndForget();
        }

        // Свойства пользователя
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
                    ((AsyncRelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (SetProperty(ref _confirmPassword, value))
                    ((AsyncRelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        // Прогресс
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

        // Режим редактирования
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        // Команды
        public ICommand CloseCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Загрузка данных пользователя и статистики прогресса
        private async Task LoadUserDataAsync()
        {
            try
            {
                // 1. Загружаем данные пользователя
                var user = await _userApi.GetByIdAsync(_userId);
                if (user != null)
                {
                    Login = user.Login;
                    Email = user.Email;
                    Username = user.Username;
                }

                // 2. Загружаем все пазлы (для расчёта общего количества)
                var allPuzzles = await _puzzleApi.GetAllAsync();
                int totalTraining = allPuzzles.Count(p => p.IsTraining);
                int totalPractice = allPuzzles.Count(p => !p.IsTraining);

                // 3. Загружаем прогресс пользователя (решённые пазлы)
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

                // 4. Вычисляем проценты
                TrainingProgress = totalTraining > 0 ? (solvedTraining * 100 / totalTraining) : 0;
                PracticeProgress = totalPractice > 0 ? (solvedPractice * 100 / totalPractice) : 0;
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка загрузки профиля: {ex.Message}");
            }
        }

        // Отмена редактирования
        private void CancelEdit()
        {
            IsEditMode = false;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            // Перезагружаем оригинальные данные
            _ = LoadUserDataAsync();
        }

        // Проверка возможности сохранения
        private bool CanSave()
        {
            if (!IsEditMode) return false;
            if (string.IsNullOrWhiteSpace(NewPassword) && string.IsNullOrWhiteSpace(ConfirmPassword))
                return true; // можно сохранить без смены пароля
            return NewPassword == ConfirmPassword && !string.IsNullOrWhiteSpace(NewPassword);
        }

        // Сохранение изменений
        private async Task SaveAsync()
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