using Hairulin_02_01.ViewModels.Base;
using System.Windows;
using System.Windows.Input;

namespace Hairulin_02_01.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;

        private string _login;
        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        public LoginViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            GoToRegisterCommand = new RelayCommand(_ => _mainViewModel.NavigateToRegisterCommand.Execute(null));
        }

        private bool CanExecuteLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin(object parameter)
        {
            // Здесь будет обращение к БД через сервис
            if (Login == "admin" && Password == "admin")
            {
                _mainViewModel.NavigateToAdminCommand.Execute(null);
            }
            else
            {
                ErrorMessage = "Неверный логин или пароль";
            }
        }
    }
}