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
            _apiService = App.Services.GetService<AuthApiService>() ?? throw new Exception("ApiService not registered");
            _navigationService = App.Services.GetService<NavigationService>() ?? throw new Exception("NavigationService not registered");

            LoginCommand = new AsyncRelayCommand(OnLoginAsync, _ => !IsBusy);
            ShowRegisterCommand = new AsyncRelayCommand(_ => _navigationService.NavigateToAsync<RegisterViewModel>());
            KeyDownCommand = new AsyncRelayCommand<KeyEventArgs>(OnKeyDownAsync, _ => !IsBusy);
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
                var response = await _apiService.LoginAsync(Login, Password);
                if (response == null)
                    return;

                if (response.IsAdmin)
                {
                    await DialogService.ShowMessage($"Добро пожаловать, {response.Username}!");
                    await _navigationService.NavigateToAsync<AdminViewModel>();
                }
                else
                {
                    await DialogService.ShowMessage($"Добро пожаловать, {response.Username}!");
                    await _navigationService.NavigateToAsync<UserViewModel>();
                }
            }
            catch (Exception ex)
            {
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

            var element = e.OriginalSource as System.Windows.UIElement;
            if (element == null) return;

            // Навигация по стрелкам вверх/вниз
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
            // Enter - переход к следующему полю или вход
            else if (e.Key == Key.Enter)
            {
                // Определяем тип элемента и его имя
                string elementName = string.Empty;
                if (element is TextBox textBox)
                    elementName = textBox.Name;
                else if (element is PasswordBox passwordBox)
                    elementName = passwordBox.Name;

                if (elementName == "txtLogin")
                {
                    // С логина переходим на пароль
                    var request = new TraversalRequest(FocusNavigationDirection.Next);
                    element.MoveFocus(request);
                }
                else if (elementName == "txtPassword" || elementName == "txtPasswordVisible")
                {
                    // На поле пароля - делаем вход
                    await Task.Delay(50);
                    await OnLoginAsync();
                }
                e.Handled = true;
            }
        }
    }
}