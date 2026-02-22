using Hairulin_02_01.Services;
using Hairulin_02_01.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hairulin_02_01.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private readonly DialogService _dialogService;
        private readonly NavigationService _navigationService;


        public string _login;
        public string _username;
        public string _email;
        
        public ICommand RegisterCommand { get; }
        public ICommand ShowLoginCommand { get; }

        public string Login
        {
            get => _login;
            set {  _login = value?.Trim() ?? string.Empty;
            OnPropertyChanged();}
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
            _dialogService = App.Services.GetService<DialogService>() ?? throw new Exception("DialogService not registered");
            _navigationService = App.Services.GetService<NavigationService>() ?? throw new Exception("NavigationService not registered");

            RegisterCommand = new RelayCommand(p => OnRegister(p));

            ShowLoginCommand = new RelayCommand(_ =>
            {
                _navigationService?.NavigateTo<LoginViewModel>();
            });
        }

        private async void OnRegister(object parameter)
        {
            var view = parameter as FrameworkElement;

            var pbPassword = view?.FindName("txtPassword") as PasswordBox;
            var pbConfirm = view?.FindName("txtConfirmPassword") as PasswordBox;

            string? password = pbPassword?.Password;
            string? confirm = pbConfirm?.Password;

            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email))
            {
                _dialogService.ShowError("Заполните все поля!");
                return;
            }

            if (password != confirm)
            {
                _dialogService.ShowError("Пароли не совпадают!");
                return;
            }

            try
            {
                var newUser = new User
                {
                    Login = Login,
                    PasswordHash = password,
                    Username = Username,
                    Email = Email
                };

                var registeredUser = await _apiService.RegisterAsync(newUser);

                if (registeredUser != null)
                    _dialogService.ShowMessage($"Регистрация успешна! Добро пожаловать, {registeredUser.Username}");

            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка регистрации: {ex.Message}");
            }
        }
    }
}