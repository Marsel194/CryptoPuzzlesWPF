using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly AuthApiService _apiService;
        private readonly NavigationService _navigationService;

        private string _login = string.Empty;

        public ICommand LoginCommand { get; }
        public ICommand ShowRegisterCommand { get; }

        public string Login
        {
            get => _login;
            set { _login = value ?? string.Empty; OnPropertyChanged(); }
        }

        public LoginViewModel()
        {
            _apiService = App.Services.GetService<AuthApiService>() ?? throw new Exception("ApiService not registered");
            _navigationService = App.Services.GetService<NavigationService>() ?? throw new Exception("NavigationService not registered");

            LoginCommand = new AsyncRelayCommand(OnLoginAsync);
            ShowRegisterCommand = new AsyncRelayCommand(_ => _navigationService.NavigateToAsync<RegisterViewModel>());
        }

        private async Task OnLoginAsync(object? parameter)
        {
            var passwordBox = parameter as PasswordBox;
            string password = passwordBox?.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(password))
            {
                DialogService.ShowError("Введите логин и пароль!");
                return;
            }

            try
            {
                var response = await _apiService.LoginAsync(Login, password);
                if (response == null)
                    return;

                if (response.IsAdmin){
                    DialogService.ShowMessage($"Добро пожаловать, {response.Username}!");
                    await _navigationService.NavigateToAsync<AdminViewModel>();
                }
                else
                {
                    DialogService.ShowMessage($"Добро пожаловать, {response.Username}!");
                    await _navigationService.NavigateToAsync<UserViewModel>();
                }
            }
            catch (Exception ex)
            {
                DialogService.ShowError($"Ошибка входа: {ex.Message}");
            }
        }
    }
}