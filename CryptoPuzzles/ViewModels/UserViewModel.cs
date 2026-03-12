using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        private readonly UserApiService _userApiService;
        private readonly UserStatisticsApiService _userStatisticsApiService;
        private readonly SessionProgressApiService _sessionProgressApiService;
        private readonly GameSessionApiService _sessionApiService;
        private readonly PuzzleApiService _puzzleApiService;
        private readonly UserSessionService _userSessionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly NavigationService _navigationService;

        private ViewModelBase? _currentSection;
        private string _username = "Пользователь";
        private int _solvedCount;
        private int _score;
        private bool _isLoading;
        private AUser? _currentUser;

        // Свойство для получения ID текущего пользователя из сессии
        private int CurrentUserId => _userSessionService.CurrentUserId ??
            throw new InvalidOperationException("Пользователь не авторизован");

        public ViewModelBase? CurrentSection
        {
            get => _currentSection;
            set => SetProperty(ref _currentSection, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public int SolvedCount
        {
            get => _solvedCount;
            set => SetProperty(ref _solvedCount, value);
        }

        public int Score
        {
            get => _score;
            set => SetProperty(ref _score, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public AUser? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public ICommand LogoutCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand StartTrainingCommand { get; }
        public ICommand StartPracticeCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand RefreshStatsCommand { get; }

        public UserViewModel(
            UserApiService userApiService,
            UserStatisticsApiService userStatisticsApiService,
            SessionProgressApiService sessionProgressApiService,
            GameSessionApiService sessionApiService,
            PuzzleApiService puzzleApiService,
            UserSessionService userSessionService,
            IServiceProvider serviceProvider)
        {
            _userApiService = userApiService;
            _userStatisticsApiService = userStatisticsApiService;
            _sessionProgressApiService = sessionProgressApiService;
            _sessionApiService = sessionApiService;
            _puzzleApiService = puzzleApiService;
            _userSessionService = userSessionService;
            _serviceProvider = serviceProvider;
            _navigationService = App.Services.GetRequiredService<NavigationService>();

            LogoutCommand = new AsyncRelayCommand(LogoutAsync);
            ToggleThemeCommand = new AsyncRelayCommand(async _ => await ThemeHelper.ToggleTheme());
            OpenProfileCommand = new AsyncRelayCommand(OpenProfileAsync);
            StartTrainingCommand = new AsyncRelayCommand(StartTrainingAsync);
            StartPracticeCommand = new AsyncRelayCommand(StartPracticeAsync);
            GoBackCommand = new AsyncRelayCommand(GoBackAsync);
            RefreshStatsCommand = new AsyncRelayCommand(LoadUserStatsAsync);

            // Загружаем данные при инициализации
            _ = LoadUserDataAsync();
        }

        private async Task LoadUserDataAsync()
        {
            try
            {
                IsLoading = true;

                // Проверяем авторизацию
                if (!_userSessionService.IsAuthenticated)
                {
                    await _navigationService.NavigateToAsync<LoginViewModel>();
                    return;
                }

                // Загружаем данные пользователя и статистику параллельно
                var userTask = _userApiService.GetByIdAsync(CurrentUserId);
                var statsTask = LoadUserStatsAsync();

                CurrentUser = await userTask;
                if (CurrentUser != null)
                {
                    Username = CurrentUser.Username;
                }

                await statsTask;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UserViewModel] Error loading data: {ex.Message}");
                await DialogService.ShowError($"Ошибка загрузки данных: {ex.Message}");

                // Если ошибка авторизации - выходим
                if (ex.Message.Contains("авторизован") || ex.Message.Contains("Unauthorized"))
                {
                    await LogoutAsync();
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadUserStatsAsync()
        {
            try
            {
                // Получаем статистику пользователя
                var userStats = await _userStatisticsApiService.GetByUserIdAsync(CurrentUserId);
                if (userStats != null)
                {
                    Score = userStats.TotalScore;
                    // Если в AUserStatistic есть поле SolvedPuzzlesCount, используем его
                    // SolvedCount = userStats.SolvedPuzzlesCount;
                }

                // Считаем решенные задачи через SessionProgressApiService
                var solvedProgress = await _sessionProgressApiService.GetAllAsync(
                    userId: CurrentUserId,
                    solved: true
                );

                // Убираем дубликаты (если одна задача решена несколько раз)
                SolvedCount = solvedProgress
                    .GroupBy(p => p.PuzzleId)
                    .Count();

                // Получаем активные сессии пользователя
                var allSessions = await _sessionApiService.GetAllAsync();
                var userActiveSessions = allSessions.Count(s =>
                    s.UserId == CurrentUserId && s.CompletedAt == null);

                // Можно добавить свойство для отображения активных сессий
                // ActiveSessions = userActiveSessions;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UserViewModel] Error loading stats: {ex.Message}");
                throw;
            }
        }

        private async Task LogoutAsync()
        {
            try
            {
                IsLoading = true;
                _userSessionService.ClearUser();
                await _navigationService.NavigateToAsync<LoginViewModel>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task OpenProfileAsync()
        {
            try
            {
                if (CurrentUser == null)
                {
                    CurrentUser = await _userApiService.GetByIdAsync(CurrentUserId);
                }

                if (CurrentUser == null) return;

                var profileVM = ActivatorUtilities.CreateInstance<UserProfileViewModel>(
                    _serviceProvider,
                    CurrentUser,
                    (Action)(() => CurrentSection = null)
                );

                CurrentSection = profileVM;
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка открытия профиля: {ex.Message}");
            }
        }

        private Task GoBackAsync()
        {
            CurrentSection = null;
            return Task.CompletedTask;
        }

        private async Task StartTrainingAsync()
        {
            try
            {
                var trainingVM = ActivatorUtilities.CreateInstance<TrainingViewModel>(
                    _serviceProvider,
                    CurrentUserId,
                    (Action)(async () =>
                    {
                        CurrentSection = null;
                        // Обновляем статистику после завершения тренировки
                        await LoadUserStatsAsync();
                    })
                );
                CurrentSection = trainingVM;
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка запуска тренировки: {ex.Message}");
            }
        }

        private async Task StartPracticeAsync()
        {
            try
            {
                var practiceVM = ActivatorUtilities.CreateInstance<PracticeViewModel>(
                    _serviceProvider,
                    CurrentUserId,
                    (Action)(async () =>
                    {
                        CurrentSection = null;
                        // Обновляем статистику после завершения практики
                        await LoadUserStatsAsync();
                    })
                );
                CurrentSection = practiceVM;
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка запуска практики: {ex.Message}");
            }
        }
    }
}