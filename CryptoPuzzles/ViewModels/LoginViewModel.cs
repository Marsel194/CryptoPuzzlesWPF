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
        private bool _isBusy;

        public AsyncRelayCommand LoginCommand { get; }
        public ICommand ShowRegisterCommand { get; }
        public AsyncRelayCommand<KeyEventArgs> KeyDownCommand { get; }

        public string Login
        {
            get => _login;
            set { _login = value ?? string.Empty; OnPropertyChanged(); }
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

        private async Task OnLoginAsync(object? parameter)
        {
            if (IsBusy) return;

            var passwordBox = parameter as PasswordBox;
            string password = passwordBox?.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(password))
            {
                await DialogService.ShowError("Введите логин и пароль!");
                return;
            }

            try
            {
                IsBusy = true;
                var response = await _apiService.LoginAsync(Login, password);
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

        private async Task OnKeyDownAsync(KeyEventArgs e)
        {
            if (IsBusy) return;

            if (e.Key == Key.Enter)
            {
                var element = e.OriginalSource as System.Windows.UIElement;

                if (element != null)
                {
                    element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                    if (element is PasswordBox passwordBox)
                    {
                        await Task.Delay(50);
                        await OnLoginAsync(passwordBox);
                    }
                }
                e.Handled = true;
            }
        }
    }
}