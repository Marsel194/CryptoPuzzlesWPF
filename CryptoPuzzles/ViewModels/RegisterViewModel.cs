using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        private bool _isEmailWarningVisible;
        private bool _isPasswordWarningVisible;
        private bool _isConfirmPasswordWarningVisible;

        public AsyncRelayCommand RegisterCommand { get; }
        public ICommand ShowLoginCommand { get; }
        public ICommand EmailTextInputCommand { get; }
        public ICommand EmailLostFocusCommand { get; }
        public ICommand PasswordLostFocusCommand { get; }
        public ICommand ConfirmPasswordLostFocusCommand { get; }
        public AsyncRelayCommand<KeyEventArgs> KeyDownCommand { get; }

        public bool IsEmailWarningVisible
        {
            get => _isEmailWarningVisible;
            set { _isEmailWarningVisible = value; OnPropertyChanged(); }
        }

        public bool IsPasswordWarningVisible
        {
            get => _isPasswordWarningVisible;
            set { _isPasswordWarningVisible = value; OnPropertyChanged(); }
        }

        public bool IsConfirmPasswordWarningVisible
        {
            get => _isConfirmPasswordWarningVisible;
            set { _isConfirmPasswordWarningVisible = value; OnPropertyChanged(); }
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
            EmailTextInputCommand = new AsyncRelayCommand<TextCompositionEventArgs>(OnTextInputEmailAsync);
            EmailLostFocusCommand = new AsyncRelayCommand<RoutedEventArgs>(OnEmailLostFocusAsync);
            PasswordLostFocusCommand = new AsyncRelayCommand<RoutedEventArgs>(OnPasswordLostFocusAsync);
            ConfirmPasswordLostFocusCommand = new AsyncRelayCommand<RoutedEventArgs>(OnConfirmPasswordLostFocusAsync);
        }

        private async Task OnKeyDownAsync(KeyEventArgs? e)
        {
            if (IsBusy || e == null) return;

            var element = e.OriginalSource as FrameworkElement;
            if (element == null) return;

            // Навигация по стрелкам вниз/вверх
            if (e.Key == Key.Down)
            {
                var request = new TraversalRequest(FocusNavigationDirection.Next);
                element.MoveFocus(request);
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                var request = new TraversalRequest(FocusNavigationDirection.Previous);
                element.MoveFocus(request);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                string elementName = GetElementName(element);

                // Если это последнее поле - регистрируемся
                if (elementName == "txtConfirmPassword" || elementName == "txtConfirmPasswordVisible")
                {
                    await Task.Delay(50);
                    await RegisterAsync();
                }
                else
                {
                    // Иначе переходим к следующему полю
                    var request = new TraversalRequest(FocusNavigationDirection.Next);
                    element.MoveFocus(request);
                }
                e.Handled = true;
            }
        }

        private string GetElementName(FrameworkElement element)
        {
            if (element is TextBox textBox)
                return textBox.Name;
            else if (element is PasswordBox passwordBox)
                return passwordBox.Name;
            else
                return string.Empty;
        }

        private async Task OnTextInputEmailAsync(TextCompositionEventArgs? e)
        {
            if (IsBusy || e == null) return;

            ValidateEmail();
            await Task.CompletedTask;
        }

        private async Task OnEmailLostFocusAsync(RoutedEventArgs? e)
        {
            ValidateEmail();
            await Task.CompletedTask;
        }

        private void ValidateEmail()
        {
            IsEmailWarningVisible = !IsValidEmail(Email);
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async Task OnPasswordLostFocusAsync(RoutedEventArgs? e)
        {
            IsPasswordWarningVisible = Password.Length < 8;
            await Task.CompletedTask;
        }

        private async Task OnConfirmPasswordLostFocusAsync(RoutedEventArgs? e)
        {
            IsConfirmPasswordWarningVisible = Password != ConfirmPassword;
            await Task.CompletedTask;
        }

        private async Task RegisterAsync(object? parameter = null)
        {
            if (IsBusy) return;

            // Проверка заполнения всех полей
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email))
            {
                await DialogService.ShowError("Заполните все поля!");
                return;
            }

            // Валидация email
            if (!IsValidEmail(Email))
            {
                IsEmailWarningVisible = true;
                await DialogService.ShowError("Введите корректный email!");
                return;
            }

            // Валидация длины пароля
            if (Password.Length < 8)
            {
                IsPasswordWarningVisible = true;
                await DialogService.ShowError("Пароль должен содержать минимум 8 символов!");
                return;
            }

            // Проверка совпадения паролей
            if (Password != ConfirmPassword)
            {
                IsConfirmPasswordWarningVisible = true;
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