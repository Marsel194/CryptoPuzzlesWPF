using CryptoPuzzles.Services;
using CryptoPuzzles.ViewModels.Base;
using CryptoPuzzles.SharedDTO;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;
using CryptoPuzzles.Services.ApiService;
using System.Windows;

namespace CryptoPuzzles.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly AuthApiService _apiService;
        private readonly NavigationService _navigationService;

        private string _login = string.Empty;
        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private bool _isBusy;

        public AsyncRelayCommand RegisterCommand { get; }
        public ICommand ShowLoginCommand { get; }
        public AsyncRelayCommand<KeyEventArgs> KeyDownCommand { get; } // Исправлено: добавлен <KeyEventArgs>

        public string Login
        {
            get => _login;
            set { _login = value?.Trim() ?? string.Empty; OnPropertyChanged(); }
        }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value?.Trim() ?? string.Empty; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                // Вместо NotifyCanExecuteChanged используем CommandManager
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public RegisterViewModel()
        {
            _apiService = App.Services.GetService<AuthApiService>() ?? throw new Exception("ApiService not registered");
            _navigationService = App.Services.GetService<NavigationService>() ?? throw new Exception("NavigationService not registered");

            RegisterCommand = new AsyncRelayCommand(RegisterAsync, _ => !IsBusy);
            ShowLoginCommand = new AsyncRelayCommand(_ => _navigationService.NavigateToAsync<LoginViewModel>());
            KeyDownCommand = new AsyncRelayCommand<KeyEventArgs>(OnKeyDownAsync, _ => !IsBusy); // Исправлено
        }

        private async Task OnKeyDownAsync(KeyEventArgs e)
        {
            if (IsBusy) return; // Добавлена проверка

            if (e.Key == Key.Enter)
            {
                var element = e.OriginalSource as FrameworkElement;
                if (element != null)
                {
                    element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                    if (element.Name == "txtConfirmPassword")
                    {
                        await Task.Delay(50);
                        await RegisterAsync();
                    }
                }
                e.Handled = true;
            }
        }

        private async Task RegisterAsync(object? parameter = null)
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email))
            {
                await DialogService.ShowError("Заполните все поля!");
                return;
            }

            if (!Email.Contains('@'))
            {
                await DialogService.ShowError("Email введен некорректно");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await DialogService.ShowError("Пароли не совпадают!");
                return;
            }

            try
            {
                IsBusy = true;
                var registerRequest = new UARegisterRequest(Login, Username, Email, Password);
                var registeredUser = await _apiService.RegisterAsync(registerRequest);

                if (registeredUser != null)
                {
                    await DialogService.ShowMessage($"Регистрация успешна! Добро пожаловать, {registeredUser.Username}");
                    await _navigationService.NavigateToAsync<LoginViewModel>();
                }
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка регистрации: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}