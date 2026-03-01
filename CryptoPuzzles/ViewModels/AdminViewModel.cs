using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;
using CryptoPuzzles.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        private readonly NavigationService _navigation;
        private readonly UserApiService _userApi;
        private readonly AdminApiService _adminApi;
        private readonly EncryptionMethodApiService _methodApi;
        private readonly PuzzleApiService _puzzleApi;
        private readonly HintApiService _hintApi;
        private readonly GameSessionApiService _sessionApi;
        private readonly TutorialApiService _tutorialApi;

        private int _totalUsers;
        private int _newUsersToday;
        private int _activeUsers;
        private int _totalAdmins;
        private int _totalMethods;
        private int _totalPuzzles;
        private int _activePuzzles;
        private int _draftPuzzles;
        private int _totalHints;
        private int _activeSessions;
        private double _avgScore;
        private int _totalTutorials;
        private int _totalSolved;
        private int _solvedToday;
        private ObservableCollection<RecentAction> _recentActions;

        public AdminViewModel()
        {
            _navigation = App.Services.GetRequiredService<NavigationService>();
            _userApi = App.Services.GetRequiredService<UserApiService>();
            _adminApi = App.Services.GetRequiredService<AdminApiService>();
            _methodApi = App.Services.GetRequiredService<EncryptionMethodApiService>();
            _puzzleApi = App.Services.GetRequiredService<PuzzleApiService>();
            _hintApi = App.Services.GetRequiredService<HintApiService>();
            _sessionApi = App.Services.GetRequiredService<GameSessionApiService>();
            _tutorialApi = App.Services.GetRequiredService<TutorialApiService>();

            NavigateToUsersCommand = new AsyncRelayCommand(() => _navigation.NavigateToAsync<UsersViewModel>());
            NavigateToAdminsCommand = new AsyncRelayCommand(() => _navigation.NavigateToAsync<AdminsViewModel>());
            NavigateToMethodsCommand = new AsyncRelayCommand(() => _navigation.NavigateToAsync<MethodsViewModel>());
            NavigateToPuzzlesCommand = new AsyncRelayCommand(() => _navigation.NavigateToAsync<PuzzlesViewModel>());
            NavigateToHintsCommand = new AsyncRelayCommand(() => _navigation.NavigateToAsync<HintsViewModel>());
            NavigateToSessionsCommand = new AsyncRelayCommand(() => _navigation.NavigateToAsync<SessionsViewModel>());
            NavigateToTutorialsCommand = new AsyncRelayCommand(() => _navigation.NavigateToAsync<TutorialsViewModel>());
            NavigateToStatisticsCommand = new AsyncRelayCommand(() => _navigation.NavigateToAsync<StatisticsViewModel>());
            ToggleThemeCommand = new AsyncRelayCommand(() => { ToggleTheme(); return Task.CompletedTask; });
            LogoutCommand = new AsyncRelayCommand(LogoutAsync);

            LoadStatsAsync();
        }

        public int TotalUsers { get => _totalUsers; set => SetProperty(ref _totalUsers, value); }
        public int NewUsersToday { get => _newUsersToday; set => SetProperty(ref _newUsersToday, value); }
        public int ActiveUsers { get => _activeUsers; set => SetProperty(ref _activeUsers, value); }
        public int TotalAdmins { get => _totalAdmins; set => SetProperty(ref _totalAdmins, value); }
        public int TotalMethods { get => _totalMethods; set => SetProperty(ref _totalMethods, value); }
        public int TotalPuzzles { get => _totalPuzzles; set => SetProperty(ref _totalPuzzles, value); }
        public int ActivePuzzles { get => _activePuzzles; set => SetProperty(ref _activePuzzles, value); }
        public int DraftPuzzles { get => _draftPuzzles; set => SetProperty(ref _draftPuzzles, value); }
        public int TotalHints { get => _totalHints; set => SetProperty(ref _totalHints, value); }
        public int ActiveSessions { get => _activeSessions; set => SetProperty(ref _activeSessions, value); }
        public double AvgScore { get => _avgScore; set => SetProperty(ref _avgScore, value); }
        public int TotalTutorials { get => _totalTutorials; set => SetProperty(ref _totalTutorials, value); }
        public int TotalSolved { get => _totalSolved; set => SetProperty(ref _totalSolved, value); }
        public int SolvedToday { get => _solvedToday; set => SetProperty(ref _solvedToday, value); }
        public ObservableCollection<RecentAction> RecentActions { get => _recentActions; set => SetProperty(ref _recentActions, value); }

        public ICommand NavigateToUsersCommand { get; }
        public ICommand NavigateToAdminsCommand { get; }
        public ICommand NavigateToMethodsCommand { get; }
        public ICommand NavigateToPuzzlesCommand { get; }
        public ICommand NavigateToHintsCommand { get; }
        public ICommand NavigateToSessionsCommand { get; }
        public ICommand NavigateToTutorialsCommand { get; }
        public ICommand NavigateToStatisticsCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand LogoutCommand { get; }

        private async Task LoadStatsAsync()
        {
            try
            {
                var users = await _userApi.GetAllAsync();
                TotalUsers = users.Count;
                NewUsersToday = users.Count(u => u.CreatedAt?.Date == DateTime.Today);
                ActiveUsers = users.Count;

                var admins = await _adminApi.GetAllAsync();
                TotalAdmins = admins.Count;

                var methods = await _methodApi.GetAllAsync();
                TotalMethods = methods.Count;

                var puzzles = await _puzzleApi.GetAllAsync();
                TotalPuzzles = puzzles.Count;
                ActivePuzzles = puzzles.Count;
                DraftPuzzles = 0;

                var hints = await _hintApi.GetAllAsync();
                TotalHints = hints.Count;

                var sessions = await _sessionApi.GetAllAsync();
                ActiveSessions = sessions.Count(s => s.CompletedAt == null);
                AvgScore = sessions.Any() ? sessions.Average(s => s.Score) : 0;

                var tutorials = await _tutorialApi.GetAllAsync();
                TotalTutorials = tutorials.Count;

                TotalSolved = sessions.Count(s => s.CompletedAt != null);
                SolvedToday = sessions.Count(s => s.CompletedAt?.Date == DateTime.Today);

                RecentActions = new ObservableCollection<RecentAction>();
            }
            catch (Exception ex)
            {
                DialogService.ShowError("Ошибка загрузки статистики: " + ex.Message);
            }
        }

        private void ToggleTheme()
        {
            ThemeHelper.ToggleTheme();
        }

        private async Task LogoutAsync()
        {
            await _navigation.NavigateToAsync<LoginViewModel>();
        }
    }

    public class RecentAction
    {
        public string User { get; set; } = "";
        public string Action { get; set; } = "";
        public DateTime Time { get; set; }
    }
}