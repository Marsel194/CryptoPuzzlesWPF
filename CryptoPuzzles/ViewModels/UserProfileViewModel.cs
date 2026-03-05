using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;
using CryptoPuzzles.ViewModels.Base;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class UserProfileViewModel : ViewModelBase
    {
        private readonly UserApiService _userApiService;
        private readonly Action _closeAction;
        private readonly int _userId;

        private string _username;
        private string _login;
        private string _email;
        private string _newPassword;
        private string _confirmPassword;
        private int _trainingProgress;
        private int _practiceProgress;
        private bool _isEditMode;

        private string _originalUsername;
        private string _originalEmail;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string NewPassword
        {
            get => _newPassword;
            set { _newPassword = value; OnPropertyChanged(); }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        public int TrainingProgress
        {
            get => _trainingProgress;
            set { _trainingProgress = value; OnPropertyChanged(); }
        }

        public int PracticeProgress
        {
            get => _practiceProgress;
            set { _practiceProgress = value; OnPropertyChanged(); }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set { _isEditMode = value; OnPropertyChanged(); }
        }

        public ICommand CloseCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public UserProfileViewModel(
       UserApiService userApiService,
       AUser user,
       Action closeAction)
        {
            _userApiService = userApiService;
            _closeAction = closeAction;
            _userId = user.Id;

            Username = user.Username;
            Login = user.Login;
            Email = user.Email;
            TrainingProgress = 65;   // заглушка
            PracticeProgress = 42;

            CloseCommand = new AsyncRelayCommand(CloseAsync);
            EditCommand = new AsyncRelayCommand(EditAsync);
            SaveCommand = new AsyncRelayCommand(SaveAsync);
            CancelCommand = new AsyncRelayCommand(CancelAsync);
        }

        private Task CloseAsync()
        {
            _closeAction?.Invoke();
            return Task.CompletedTask;
        }

        private Task EditAsync()
        {
            _originalUsername = Username;
            _originalEmail = Email;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            IsEditMode = true;
            return Task.CompletedTask;
        }

        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
            {
                await DialogService.ShowError("Введите корректный email, содержащий '@'.");
                return;
            }

            bool changePassword = !string.IsNullOrWhiteSpace(NewPassword);
            if (changePassword)
            {
                if (NewPassword != ConfirmPassword)
                {
                    await DialogService.ShowError("Введенные пароли не совпадают.");
                    return;
                }
                if (NewPassword.Length < 6)
                {
                    await DialogService.ShowError("Пароль должен содержать не менее 6 символов.");
                    return;
                }
            }

            try
            {
                var updateDto = new AUserUpdate(
                    Id: _userId,
                    Login: this.Login,
                    Username: this.Username,
                    Email: this.Email,
                    Password: changePassword ? this.NewPassword : null
                );

                await _userApiService.UpdateAsync(_userId, updateDto);

                _originalUsername = Username;
                _originalEmail = Email;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;
                IsEditMode = false;
                await DialogService.ShowMessage("Данные успешно сохранены.");
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private Task CancelAsync()
        {
            Username = _originalUsername;
            Email = _originalEmail;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            IsEditMode = false;
            return Task.CompletedTask;
        }
    }
}