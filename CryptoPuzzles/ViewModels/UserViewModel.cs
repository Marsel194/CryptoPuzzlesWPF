using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;
using CryptoPuzzles.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        private readonly UserApiService _userApiService;
        private readonly IServiceProvider _serviceProvider;
        private readonly NavigationService _navigationService;
        private readonly int _userId = 1; // заглушка, потом брать из сессии

        private ViewModelBase? _currentSection;
        public ViewModelBase? CurrentSection
        {
            get => _currentSection;
            set => SetProperty(ref _currentSection, value);
        }

        public int SolvedCount { get; set; } = 12;
        public int Score { get; set; } = 1250;

        public ICommand LogoutCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand StartTrainingCommand { get; }
        public ICommand StartPracticeCommand { get; }
        public ICommand GoBackCommand { get; }

        public UserViewModel(UserApiService userApiService, IServiceProvider serviceProvider)
        {
            _userApiService = userApiService;
            _serviceProvider = serviceProvider;
            _navigationService = App.Services.GetRequiredService<NavigationService>();
            LogoutCommand = new AsyncRelayCommand(async _ => await _navigationService.NavigateToAsync<LoginViewModel>());
            ToggleThemeCommand = new AsyncRelayCommand(async _ => await ThemeHelper.ToggleTheme());
            OpenProfileCommand = new AsyncRelayCommand(OpenProfileAsync);
            StartTrainingCommand = new AsyncRelayCommand(StartTrainingAsync);
            StartPracticeCommand = new AsyncRelayCommand(StartPracticeAsync);
            GoBackCommand = new AsyncRelayCommand(GoBackAsync);
        }

        private async Task OpenProfileAsync()
        {
            var user = await _userApiService.GetByIdAsync(_userId);
            if (user == null) return;

            var profileVM = ActivatorUtilities.CreateInstance<UserProfileViewModel>(
                _serviceProvider,
                user,
                (Action)(() => CurrentSection = null)
            );

            CurrentSection = profileVM;
        }

        private Task GoBackAsync()
        {
            CurrentSection = null;
            return Task.CompletedTask;
        }

        private Task StartTrainingAsync() { /* ... */ return Task.CompletedTask; }
        private Task StartPracticeAsync() { /* ... */ return Task.CompletedTask; }
    }
}