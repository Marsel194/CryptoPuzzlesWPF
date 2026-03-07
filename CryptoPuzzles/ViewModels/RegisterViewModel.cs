using CryptoPuzzles.Services;
using CryptoPuzzles.ViewModels.Base;
using CryptoPuzzles.Shared;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;
using CryptoPuzzles.Services.ApiService;
using System.Windows;
using DocumentFormat.OpenXml.Office2013.Drawing.Chart;

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
        private bool _isEmailWarningVisible = false;

        public AsyncRelayCommand RegisterCommand { get; }
        public ICommand ShowLoginCommand { get; }
        public ICommand TextInputEmailCommand { get; }
        public AsyncRelayCommand<KeyEventArgs> KeyDownCommand { get; }

        public bool IsEmailWarningVisiible
        {
            get => _isEmailWarningVisible;
            set { _isEmailWarningVisible = value;
                OnPropertyChanged();
            }
        }

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
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public RegisterViewModel()
        {
            _apiService = App.Services.GetService<AuthApiService>() ?? throw new Exception("ApiService not registered");
            _navigationService = App.Services.GetService<NavigationService>() ?? throw new Exception("NavigationService not registered");

            RegisterCommand = new AsyncRelayCommand(RegisterAsync, _ => !IsBusy);
            ShowLoginCommand = new AsyncRelayCommand(_ => _navigationService.NavigateToAsync<LoginViewModel>());
            KeyDownCommand = new AsyncRelayCommand<KeyEventArgs>(OnKeyDownAsync, _ => !IsBusy);
            TextInputEmailCommand = new AsyncRelayCommand<TextCompositionEventArgs>(OnTextInputEmailAsync);
        }

        private async Task OnKeyDownAsync(KeyEventArgs? e)
        {
            if (IsBusy || e == null) return;

            if (e.Key == Key.Enter)
            {
                if (e.OriginalSource is FrameworkElement element)
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
        private async Task OnTextInputEmailAsync(TextCompositionEventArgs? e)
        {
            if (IsBusy || e == null) return;

            if (e.Text == "@")
            {
                IsEmailWarningVisiible = true;
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
                IsEmailWarningVisiible = false;
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