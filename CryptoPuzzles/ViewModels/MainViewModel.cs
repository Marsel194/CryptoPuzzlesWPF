using CryptoPuzzles.ViewModels.Base;
using System.Windows.Input;
using CryptoPuzzles.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoPuzzles.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public ICommand NavigateToLoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }
        public ICommand NavigateToAdminCommand { get; }

        public MainViewModel()
        {
            var services = App.Services
                ?? throw new InvalidOperationException("Services not initialized");

            var navigationService = services.GetRequiredService<NavigationService>();

            navigationService.OnViewChanged += vm => CurrentView = vm;

            CurrentView = services.GetRequiredService<LoginViewModel>();

            NavigateToLoginCommand = new AsyncRelayCommand(async _ =>
            {
                CurrentView = services.GetRequiredService<LoginViewModel>();
                await Task.CompletedTask;
            });

            NavigateToRegisterCommand = new AsyncRelayCommand(async _ =>
            {
                await navigationService.NavigateToAsync<RegisterViewModel>();
            });

            NavigateToAdminCommand = new AsyncRelayCommand(async _ =>
            {
                CurrentView = services.GetRequiredService<AdminViewModel>();
                await Task.CompletedTask;
            });
        }
    }
}