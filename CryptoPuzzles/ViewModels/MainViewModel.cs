using CryptoPuzzles.Services;
using CryptoPuzzles.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand NavigateToLoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }
        public ICommand NavigateToAdminCommand { get; }

        public MainViewModel()
        {
            if (App.Services == null)
                throw new InvalidOperationException("Services not initialized");

            var navigationService = App.Services.GetService<NavigationService>();

            navigationService?.OnViewChanged += (newView) =>
            {
                CurrentView = newView;
            };

            var loginVM = App.Services.GetService<AdminsViewModel>();
            CurrentView = loginVM ?? throw new Exception("LoginViewModel not registered");

            NavigateToLoginCommand = new RelayCommand(_ =>
            {
                var loginVM = App.Services.GetService<LoginViewModel>();
                if (loginVM != null)
                    CurrentView = loginVM;
            });

            NavigateToRegisterCommand = new RelayCommand(_ =>
            {
                navigationService?.NavigateTo<RegisterViewModel>();
            });

            NavigateToAdminCommand = new RelayCommand(_ =>
            {
                var adminVM = App.Services.GetService<AdminViewModel>();
                if (adminVM != null)
                    CurrentView = adminVM;
            });
        }
    }
}