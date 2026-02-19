using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hairulin_02_01
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private UserControl _currentView;

        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        // Команды для навигации
        public ICommand ShowLoginCommand { get; }
        public ICommand ShowRegisterCommand { get; }
        public ICommand ShowMainAppCommand { get; }
        public ICommand LogoutCommand { get; }


        public MainViewModel()
        {
            ShowLoginCommand = new RelayCommand(ShowLogin);
            ShowRegisterCommand = new RelayCommand(ShowRegister);
            ShowMainAppCommand = new RelayCommand(ShowMainApp);
            LogoutCommand = new RelayCommand(Logout);

            // Стартуем с экрана логина
            ShowLogin();
        }

        public void ShowLogin() => CurrentView = new LoginView();
        public void ShowRegister() => CurrentView = new RegisterView();
        public void ShowMainApp() => CurrentView = new MainAppView();
        public void Logout() => ShowLogin();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}