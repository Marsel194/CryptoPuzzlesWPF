using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly AdminStatsService _statsService;
        private readonly IAuthService _authService;
        private readonly AdminApiService _adminApiService;

        private int _totalUsers;
        private int _newUsersToday;
        private int _activeUsers;
        private int _totalAdmins;
        private int _totalMethods;
        private int _totalPuzzles;
        private int _activePuzzles;
        private int _totalHints;
        private int _activeSessions;
        private double _avgScore;
        private int _totalTutorials;
        private int _totalSolved;
        private int _solvedToday;
        private int _totalDifficulties;
        private bool _isInitialized;

        private ViewModelBase? _currentSection;
        public ViewModelBase? CurrentSection
        {
            get => _currentSection;
            set => SetProperty(ref _currentSection, value);
        }

        private string _username = "Администратор";
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public ICommand NavigateToUsersCommand { get; }
        public ICommand NavigateToAdminsCommand { get; }
        public ICommand NavigateToMethodsCommand { get; }
        public ICommand NavigateToPuzzlesCommand { get; }
        public ICommand NavigateToHintsCommand { get; }
        public ICommand NavigateToSessionsCommand { get; }
        public ICommand NavigateToTutorialsCommand { get; }
        public ICommand NavigateToStatisticsCommand { get; }
        public ICommand NavigateToDifficultiesCommand { get; }
        public ICommand NavigateToSessionProgressCommand { get; }
        public ICommand NavigateToUserStatisticsCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand LoadStatsCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand GoBackCommand { get; }

        public AdminViewModel()
        {
            _statsService = App.Services.GetRequiredService<AdminStatsService>();
            _navigationService = App.Services.GetRequiredService<NavigationService>();
            _authService = App.Services.GetRequiredService<IAuthService>();
            _adminApiService = App.Services.GetRequiredService<AdminApiService>();

            NavigateToUsersCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<UsersViewModel>());
            NavigateToAdminsCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<AdminsViewModel>());
            NavigateToMethodsCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<MethodsViewModel>());
            NavigateToPuzzlesCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<PuzzlesViewModel>());
            NavigateToHintsCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<HintsViewModel>());
            NavigateToSessionsCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<SessionsViewModel>());
            NavigateToTutorialsCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<TutorialsViewModel>());
            NavigateToStatisticsCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<StatisticsViewModel>());
            NavigateToDifficultiesCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<DifficultiesViewModel>());
            NavigateToSessionProgressCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<SessionProgressViewModel>());
            NavigateToUserStatisticsCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<UserStatisticsViewModel>());

            ToggleThemeCommand = new AsyncRelayCommand(async _ => await ThemeHelper.ToggleTheme());
            LogoutCommand = new AsyncRelayCommand(LogoutAsync);
            LoadStatsCommand = new AsyncRelayCommand(async _ => await LoadStatsAsync());
            OpenProfileCommand = new AsyncRelayCommand(OpenProfileAsync);
            GoBackCommand = new AsyncRelayCommand(_ => { CurrentSection = null; return Task.CompletedTask; });

            // Загружаем данные при создании ViewModel
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            if (_isInitialized) return;

            try
            {
                await Task.WhenAll(
                    LoadAdminDataAsync(),
                    LoadStatsAsync()
                );
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка при загрузке данных: {ex.Message}");
            }
            finally
            {
                _isInitialized = true;
            }
        }

        private async Task LoadAdminDataAsync()
        {
            try
            {
                var admin = _authService.CurrentAdmin;
                if (admin == null)
                {
                    // Если в сессии нет админа, пробуем загрузить по ID из токена
                    // Здесь должна быть логика получения ID из токена
                    return;
                }

                Username = $"{admin.FirstName} {admin.LastName}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AdminViewModel] Error loading admin data: {ex.Message}");
            }
        }

        private async Task OpenProfileAsync(object? parameter = null)
        {
            try
            {
                var admin = _authService.CurrentAdmin;
                if (admin == null)
                {
                    // Пробуем получить ID из UserSessionService или другого источника
                    // Например, если есть метод получения ID текущего администратора
                    await DialogService.ShowError("Не удалось загрузить данные профиля");
                    return;
                }

                var profileVM = ActivatorUtilities.CreateInstance<AdminProfileViewModel>(
                    App.Services,
                    admin,
                    (Action)(() => CurrentSection = null)
                );

                CurrentSection = profileVM;

                // Дополнительно загружаем свежие данные с сервера
                await profileVM.LoadAdminDataAsync();
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка загрузки профиля: {ex.Message}");
            }
        }

        private async Task LogoutAsync()
        {
            try
            {
                _authService.Clear();
                await _navigationService.NavigateToAsync<LoginViewModel>();
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка при выходе: {ex.Message}");
            }
        }

        public int TotalUsers { get => _totalUsers; set => SetProperty(ref _totalUsers, value); }
        public int NewUsersToday { get => _newUsersToday; set => SetProperty(ref _newUsersToday, value); }
        public int ActiveUsers { get => _activeUsers; set => SetProperty(ref _activeUsers, value); }
        public int TotalAdmins { get => _totalAdmins; set => SetProperty(ref _totalAdmins, value); }
        public int TotalMethods { get => _totalMethods; set => SetProperty(ref _totalMethods, value); }
        public int TotalPuzzles { get => _totalPuzzles; set => SetProperty(ref _totalPuzzles, value); }
        public int ActivePuzzles { get => _activePuzzles; set => SetProperty(ref _activePuzzles, value); }
        public int TotalHints { get => _totalHints; set => SetProperty(ref _totalHints, value); }
        public int ActiveSessions { get => _activeSessions; set => SetProperty(ref _activeSessions, value); }
        public double AvgScore { get => _avgScore; set => SetProperty(ref _avgScore, value); }
        public int TotalTutorials { get => _totalTutorials; set => SetProperty(ref _totalTutorials, value); }
        public int TotalSolved { get => _totalSolved; set => SetProperty(ref _totalSolved, value); }
        public int SolvedToday { get => _solvedToday; set => SetProperty(ref _solvedToday, value); }
        public int TotalDifficulties { get => _totalDifficulties; set => SetProperty(ref _totalDifficulties, value); }

        private async Task LoadStatsAsync()
        {
            try
            {
                var stats = await _statsService.LoadStatsAsync();

                TotalUsers = stats.TotalUsers;
                NewUsersToday = stats.NewUsersToday;
                ActiveUsers = stats.ActiveUsers;
                TotalAdmins = stats.TotalAdmins;
                TotalMethods = stats.TotalMethods;
                TotalPuzzles = stats.TotalPuzzles;
                ActivePuzzles = stats.ActivePuzzles;
                TotalHints = stats.TotalHints;
                ActiveSessions = stats.ActiveSessions;
                AvgScore = stats.AvgScore;
                TotalTutorials = stats.TotalTutorials;
                TotalSolved = stats.TotalSolved;
                SolvedToday = stats.SolvedToday;
                TotalDifficulties = stats.TotalDifficulties;

                if (!string.IsNullOrEmpty(stats.ErrorMessage))
                    await DialogService.ShowError(stats.ErrorMessage);
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка загрузки статистики: {ex.Message}");
            }
        }
    }

    public class RecentAction
    {
        public string User { get; set; } = "";
        public string Action { get; set; } = "";
        public DateTime Time { get; set; }
    }
}