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
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;

        public ICommand RegisterCommand { get; }
        public ICommand ShowLoginCommand { get; }

        public string Login
        {
            get => _login;
            set{  _login = value?.Trim() ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => _username;
            set{  _username = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set{  _email = value?.Trim() ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set{  _password = value;
                OnPropertyChanged();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set{  _confirmPassword = value;
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
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email))
            {
                DialogService.ShowError("Заполните все поля!");
                return;
            }

            if (Password != ConfirmPassword)
            {
                DialogService.ShowError("Пароли не совпадают!");
                return;
            }

            try
            {
                var registerRequest = new UARegisterRequest(Login, Username, Email, Password);
                var registeredUser = await _apiService.RegisterAsync(registerRequest);  // Now passes request with password

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