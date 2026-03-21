using CryptoPuzzles.Client.Services;
using CryptoPuzzles.Client.Services.ApiService;
using CryptoPuzzles.Client.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CryptoPuzzles.Client.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly AuthApiService _apiService;
        private readonly NavigationService _navigationService;
        private readonly UserSessionService _userSessionService;
        private readonly IAuthService _authService;
        private readonly AdminApiService _adminApiService;

        private string _login = string.Empty;
        private string _password = string.Empty;
        private bool _isBusy;

        public AsyncRelayCommand LoginCommand { get; }
        public ICommand ShowRegisterCommand { get; }
        public AsyncRelayCommand<KeyEventArgs> KeyDownCommand { get; }

        public string Login
        {
            get => _login;
            set { _login = value ?? string.Empty; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public LoginViewModel()
        {
            _apiService = App.Services.GetRequiredService<AuthApiService>();
            _navigationService = App.Services.GetRequiredService<NavigationService>();
            _userSessionService = App.Services.GetRequiredService<UserSessionService>();
            _authService = App.Services.GetRequiredService<IAuthService>();
            _adminApiService = App.Services.GetRequiredService<AdminApiService>();

            LoginCommand = new AsyncRelayCommand(OnLoginAsync, _ => !IsBusy);
            ShowRegisterCommand = new AsyncRelayCommand(_ => _navigationService.NavigateToAsync<RegisterViewModel>());
            KeyDownCommand = new AsyncRelayCommand<KeyEventArgs>(OnKeyDownAsync, _ => !IsBusy);

            _ = CheckConnection();
        }

        private async Task CheckConnection()
        {
            try
            {
                _ = await _adminApiService.GetAllAsync();
            }
            catch (OperationCanceledException)
            {
                await DialogService.ShowError("Сервер не отвечает. Проверьте подключение.");
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка подключения: {ex.Message}");
            }
        }

        private async Task OnLoginAsync(object? parameter = null)
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                await DialogService.ShowError("Введите логин и пароль!");
                return;
            }

            try
            {
                IsBusy = true;
                Debug.WriteLine($"Login attempt with: {Login}");

                var response = await _apiService.LoginAsync(Login, Password);
                if (response == null)
                {
                    Debug.WriteLine("Login response is NULL");
                    return;
                }

                Debug.WriteLine($"Login success - UserId: {response.Id}, IsAdmin: {response.IsAdmin}, Token exists: {!string.IsNullOrEmpty(response.Token)}");
                Debug.WriteLine($"Token value: {(response.Token?.Length > 0 ? response.Token.Substring(0, Math.Min(30, response.Token.Length)) + "..." : "EMPTY")}");

                _userSessionService.SetUser(
                    userId: response.Id,
                    login: response.Login,
                    username: response.Username,
                    isAdmin: response.IsAdmin,
                    token: response.Token
                );

                Debug.WriteLine("User set in session service, attempting navigation...");

                if (response.IsAdmin)
                {
                    var admin = await _adminApiService.GetByIdAsync(response.Id);
                    if (admin == null)
                    {
                        Debug.WriteLine("Admin not found");
                        return;
                    }

                    _authService.SetCurrentAdmin(admin);
                    await _navigationService.NavigateToAsync<AdminViewModel>();
                }
                else
                    await _navigationService.NavigateToAsync<UserViewModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Login error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                await DialogService.ShowError($"Ошибка входа: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnKeyDownAsync(KeyEventArgs? e)
        {
            if (IsBusy || e == null) return;

            if (e.OriginalSource is not UIElement element) return;

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
                string elementName = string.Empty;
                if (element is TextBox textBox)
                    elementName = textBox.Name;
                else if (element is PasswordBox passwordBox)
                    elementName = passwordBox.Name;

                if (elementName == "txtLogin")
                {
                    var request = new TraversalRequest(FocusNavigationDirection.Next);
                    element.MoveFocus(request);
                }
                else if (elementName == "txtPassword" || elementName == "txtPasswordVisible")
                {
                    await Task.Delay(50);
                    await OnLoginAsync();
                }
                e.Handled = true;
            }
        }
    }
}