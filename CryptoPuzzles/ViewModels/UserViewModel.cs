using CryptoPuzzles.ViewModels.Base;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        // Свойства пользователя
        private string _username = "Алексей";
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _login = "alexey";
        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        private string _email = "alexey@example.com";
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        // Прогресс (в процентах)
        private int _trainingProgress = 65; // 65%
        public int TrainingProgress
        {
            get => _trainingProgress;
            set { _trainingProgress = value; OnPropertyChanged(); }
        }

        private int _practiceProgress = 42; // 42%
        public int PracticeProgress
        {
            get => _practiceProgress;
            set { _practiceProgress = value; OnPropertyChanged(); }
        }

        // Состояние профиля и редактирования
        private bool _isProfileOpen;
        public bool IsProfileOpen
        {
            get => _isProfileOpen;
            set { _isProfileOpen = value; OnPropertyChanged(); }
        }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set { _isEditMode = value; OnPropertyChanged(); }
        }

        // Команды
        public ICommand LogoutCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand CloseProfileCommand { get; }
        public ICommand StartTrainingCommand { get; }
        public ICommand StartPracticeCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public UserViewModel()
        {
            LogoutCommand = new AsyncRelayCommand(async _ => await Logout());
            ToggleThemeCommand = new AsyncRelayCommand(async _ => await ToggleTheme());
            OpenProfileCommand = new AsyncRelayCommand(async _ => await OpenProfile());
            CloseProfileCommand = new AsyncRelayCommand(async _ => await CloseProfile());
            StartTrainingCommand = new AsyncRelayCommand(async _ => await StartTraining());
            StartPracticeCommand = new AsyncRelayCommand(async _ => await StartPractice());
            EditCommand = new AsyncRelayCommand(async _ => await Edit());
            SaveCommand = new AsyncRelayCommand(async _ => await Save());
            CancelCommand = new AsyncRelayCommand(async _ => await Cancel());
        }

        private async Task Logout() { /* заглушка */ }
        private async Task ToggleTheme() { /* заглушка */ }
        private async Task OpenProfile() => IsProfileOpen = true;
        private async Task CloseProfile() => IsProfileOpen = false;
        private async Task StartTraining() { /* заглушка */ }
        private async Task StartPractice() { /* заглушка */ }
        private async Task Edit() => IsEditMode = true;
        private async Task Save()
        {
            // здесь логика сохранения
            IsEditMode = false;
        }
        private async Task Cancel() => IsEditMode = false;
    }
}