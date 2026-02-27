using CryptoPuzzles.Services;
using CryptoPuzzles.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private readonly DialogService _dialogService;
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
            _apiService = App.Services.GetService<ApiService>() ?? throw new Exception("ApiService not registered");
            _dialogService = App.Services.GetService<DialogService>() ?? throw new Exception("DialogService not registered");
            _navigationService = App.Services.GetService<NavigationService>() ?? throw new Exception("NavigationService not registered");

            LoginCommand = new RelayCommand(OnLogin);
            ShowRegisterCommand = new RelayCommand(_ => _navigationService.NavigateTo<RegisterViewModel>());
        }

        private async void OnLogin(object? parameter)
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
                    _navigationService.NavigateTo<AdminViewModel>();
                }
                else
                {
                    DialogService.ShowMessage($"Добро пожаловать, {response.Username}!");
                    _navigationService.NavigateTo<UserViewModel>();
                }
            }
            catch (Exception ex)
            {
                DialogService.ShowError($"Ошибка входа: {ex.Message}");
            }
        }
    }
}