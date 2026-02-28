using CryptoPuzzles.Services;
using CryptoPuzzles.ViewModels.Base;
using CryptoPuzzles.SharedDTO;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private readonly NavigationService _navigationService;

        private string _login = string.Empty;
        private string _username = string.Empty;
        private string _email = string.Empty;

        public ICommand RegisterCommand { get; }
        public ICommand ShowLoginCommand { get; }

        public string Login
        {
            get => _login;
            set
            {
                _login = value?.Trim() ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value?.Trim() ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public RegisterViewModel()
        {
            _apiService = App.Services.GetService<ApiService>() ?? throw new Exception("ApiService not registered");
            _navigationService = App.Services.GetService<NavigationService>() ?? throw new Exception("NavigationService not registered");

            RegisterCommand = new AsyncRelayCommand(async _ =>
            {
                await RegisterAsync();
            });
            ShowLoginCommand = new AsyncRelayCommand(_ => _navigationService.NavigateToAsync<LoginViewModel>());
        }

        private async Task RegisterAsync(object? parameter = null)
        {
            var view = parameter as FrameworkElement;
            var pbPassword = view?.FindName("txtPassword") as PasswordBox;
            var pbConfirm = view?.FindName("txtConfirmPassword") as PasswordBox;

            string? password = pbPassword?.Password;
            string? confirm = pbConfirm?.Password;

            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email))
            {
                DialogService.ShowError("Заполните все поля!");
                return;
            }

            if (password != confirm)
            {
                DialogService.ShowError("Пароли не совпадают!");
                return;
            }

            try
            {
                var newUser = new UUser(Login, Username, Email);
                var registeredUser = await _apiService.RegisterAsync(newUser);

                if (registeredUser != null)
                    DialogService.ShowMessage($"Регистрация успешна! Добро пожаловать, {registeredUser.Username}");
            }
            catch (Exception ex)
            {
                DialogService.ShowError($"Ошибка регистрации: {ex.Message}");
            }
        }
    }
}