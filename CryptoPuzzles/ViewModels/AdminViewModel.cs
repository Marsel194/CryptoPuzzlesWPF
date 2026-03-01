using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.ViewModels.Base;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        private readonly AdminNavigationService _adminNavigation;
        private readonly NavigationService _navigation;

        private readonly UserApiService? _userApi;
        private readonly AdminApiService? _adminApi;
        private readonly EncryptionMethodApiService? _methodApi;
        private readonly PuzzleApiService? _puzzleApi;
        private readonly HintApiService? _hintApi;
        private readonly GameSessionApiService? _sessionApi;
        private readonly TutorialApiService? _tutorialApi;
        private readonly AdminLayoutViewModel _layout;

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

        public AdminViewModel(AdminNavigationService adminNavigation)
        {
            _adminNavigation = adminNavigation;

            NavigateToUsersCommand = new AsyncRelayCommand(_ => { _adminNavigation.NavigateTo<UsersViewModel>(); return Task.CompletedTask; });
            NavigateToAdminsCommand = new AsyncRelayCommand(_ => { _adminNavigation.NavigateTo<AdminsViewModel>(); return Task.CompletedTask; });
            NavigateToMethodsCommand = new AsyncRelayCommand(_ => { _adminNavigation.NavigateTo<MethodsViewModel>(); return Task.CompletedTask; });
            NavigateToPuzzlesCommand = new AsyncRelayCommand(_ => { _adminNavigation.NavigateTo<PuzzlesViewModel>(); return Task.CompletedTask; });
            NavigateToHintsCommand = new AsyncRelayCommand(_ => { _adminNavigation.NavigateTo<HintsViewModel>(); return Task.CompletedTask; });
            NavigateToSessionsCommand = new AsyncRelayCommand(_ => { _adminNavigation.NavigateTo<SessionsViewModel>(); return Task.CompletedTask; });
            NavigateToTutorialsCommand = new AsyncRelayCommand(_ => { _adminNavigation.NavigateTo<TutorialsViewModel>(); return Task.CompletedTask; });
            NavigateToStatisticsCommand = new AsyncRelayCommand(_ => { _adminNavigation.NavigateTo<StatisticsViewModel>(); return Task.CompletedTask; });
            ToggleThemeCommand = new AsyncRelayCommand(async _ => await ThemeHelper.ToggleTheme());
            //LogoutCommand = new AsyncRelayCommand(async _ => );

            _ = LoadStatsAsync();
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
                if (_userApi != null)
                {
                    var users = await _userApi.GetAllAsync().ConfigureAwait(false);
                    TotalUsers = users.Count;
                    NewUsersToday = users.Count(u => u.CreatedAt?.Date == DateTime.Today);
                    ActiveUsers = users.Count;
                }

                if (_adminApi != null)
                {
                    var admins = await _adminApi.GetAllAsync();
                    TotalAdmins = admins.Count;
                }

                if (_methodApi != null)
                {
                    var methods = await _methodApi.GetAllAsync();
                    TotalMethods = methods.Count;
                }

                if (_puzzleApi != null)
                {
                    var puzzles = await _puzzleApi.GetAllAsync();
                    TotalPuzzles = puzzles.Count;
                    ActivePuzzles = puzzles.Count;
                }

                if (_hintApi != null)
                {
                    var hints = await _hintApi.GetAllAsync();
                    TotalHints = hints.Count;
                }

                if (_sessionApi != null)
                {
                    var sessions = await _sessionApi.GetAllAsync();
                    ActiveSessions = sessions.Count(s => s.CompletedAt == null);
                    AvgScore = sessions.Any() ? sessions.Average(s => s.Score) : 0;
                    TotalSolved = sessions.Count(s => s.CompletedAt != null);
                    SolvedToday = sessions.Count(s => s.CompletedAt?.Date == DateTime.Today);
                }

                if (_tutorialApi != null)
                {
                    var tutorials = await _tutorialApi.GetAllAsync();
                    TotalTutorials = tutorials.Count;
                }

                DraftPuzzles = 0;
                RecentActions = [];
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => DialogService.ShowError("Ошибка загрузки статистики: " + ex.Message));
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