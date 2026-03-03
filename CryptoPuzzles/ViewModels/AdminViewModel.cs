using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly AdminStatsService _statsService;

        // Отдельные поля для каждого значения статистики
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
        private ObservableCollection<RecentAction> _recentActions = new();

        public AdminViewModel()
        {
            _statsService = App.Services.GetRequiredService<AdminStatsService>();
            _navigationService = App.Services.GetRequiredService<NavigationService>();

            // Навигационные команды
            NavigateToUsersCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<UsersViewModel>());
            NavigateToAdminsCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<AdminsViewModel>());
            NavigateToMethodsCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<MethodsViewModel>());
            NavigateToPuzzlesCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<PuzzlesViewModel>());
            NavigateToHintsCommand = new AsyncRelayCommand(async() => await _navigationService.NavigateToAsync<HintsViewModel>());
            NavigateToSessionsCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<SessionsViewModel>());
            NavigateToTutorialsCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<TutorialsViewModel>());
            NavigateToStatisticsCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<StatisticsViewModel>());
            NavigateToDifficultiesCommand = new AsyncRelayCommand(async () => await _navigationService.NavigateToAsync<DifficultiesViewModel>());

            // Системные команды
            ToggleThemeCommand = new AsyncRelayCommand(async _ => await ThemeHelper.ToggleTheme());
            LogoutCommand = new AsyncRelayCommand(async _ => await _navigationService.NavigateToAsync<LoginViewModel>());
            LoadStatsCommand = new AsyncRelayCommand(async _ => await LoadStatsAsync());

            _ = LoadStatsAsync();
        }

        // Команды
        public ICommand NavigateToUsersCommand { get; }
        public ICommand NavigateToAdminsCommand { get; }
        public ICommand NavigateToMethodsCommand { get; }
        public ICommand NavigateToPuzzlesCommand { get; }
        public ICommand NavigateToHintsCommand { get; }
        public ICommand NavigateToSessionsCommand { get; }
        public ICommand NavigateToTutorialsCommand { get; }
        public ICommand NavigateToStatisticsCommand { get; }
        public ICommand NavigateToDifficultiesCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand LoadStatsCommand { get; }

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

        public ObservableCollection<RecentAction> RecentActions
        {
            get => _recentActions;
            set => SetProperty(ref _recentActions, value);
        }

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