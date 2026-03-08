using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;
using System.Diagnostics;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class AdminProfileViewModel : ViewModelBase
    {
        private readonly AdminApiService _adminApiService;
        private readonly Action _closeAction;
        private readonly int _adminId;

        private string _login = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _middleName = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;
        private bool _isEditMode;
        private bool _isLoading;

        private string _originalFirstName = string.Empty;
        private string _originalLastName = string.Empty;
        private string _originalMiddleName = string.Empty;

        public string Login
        {
            get => _login;
            private set => SetProperty(ref _login, value);
        }

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string MiddleName
        {
            get => _middleName;
            set => SetProperty(ref _middleName, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                if (_isEditMode != value)
                {
                    _isEditMode = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                    Debug.WriteLine($"[AdminProfile] IsEditMode changed to: {value}");
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand CloseCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AdminProfileViewModel(AdminApiService adminApiService, AAdmin admin, Action closeAction)
        {
            Debug.WriteLine("==========================================");
            Debug.WriteLine("[AdminProfile] CONSTRUCTOR START");

            _adminApiService = adminApiService;
            _closeAction = closeAction;
            _adminId = admin.Id;

            // Инициализация данными администратора
            Login = admin.Login;
            FirstName = admin.FirstName;
            LastName = admin.LastName;
            MiddleName = admin.MiddleName ?? string.Empty;

            _originalFirstName = admin.FirstName;
            _originalLastName = admin.LastName;
            _originalMiddleName = admin.MiddleName ?? string.Empty;

            Debug.WriteLine($"[AdminProfile] Admin ID: {_adminId}");
            Debug.WriteLine($"[AdminProfile] Login: {Login}");
            Debug.WriteLine($"[AdminProfile] Name: {FirstName} {LastName} {MiddleName}");

            // Команды с AsyncRelayCommand
            CloseCommand = new AsyncRelayCommand(CloseAsync);
            EditCommand = new AsyncRelayCommand(EditAsync, () => !IsLoading && !IsEditMode);
            SaveCommand = new AsyncRelayCommand(SaveAsync, () => !IsLoading && IsEditMode);
            CancelCommand = new AsyncRelayCommand(CancelAsync, () => !IsLoading && IsEditMode);

            Debug.WriteLine("[AdminProfile] CONSTRUCTOR END");
            Debug.WriteLine("==========================================");
        }

        private Task CloseAsync()
        {
            Debug.WriteLine("[AdminProfile] Close executed");
            _closeAction?.Invoke();
            return Task.CompletedTask;
        }

        private Task EditAsync()
        {
            Debug.WriteLine("==========================================");
            Debug.WriteLine("[AdminProfile] Edit EXECUTED");

            _originalFirstName = FirstName;
            _originalLastName = LastName;
            _originalMiddleName = MiddleName;

            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;

            IsEditMode = true;

            Debug.WriteLine($"[AdminProfile] Edit completed, IsEditMode = {IsEditMode}");
            Debug.WriteLine("==========================================");

            return Task.CompletedTask;
        }

        private async Task SaveAsync()
        {
            Debug.WriteLine("==========================================");
            Debug.WriteLine("[AdminProfile] Save EXECUTED");

            // Валидация
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                await DialogService.ShowError("Имя не может быть пустым.");
                return;
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                await DialogService.ShowError("Фамилия не может быть пустой.");
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
                IsLoading = true;
                Debug.WriteLine("[AdminProfile] Sending update to API...");

                var updateDto = new AAdminUpdate(
                    Id: _adminId,
                    Login: this.Login,
                    FirstName: this.FirstName,
                    LastName: this.LastName,
                    MiddleName: string.IsNullOrWhiteSpace(this.MiddleName) ? null : this.MiddleName,
                    Password: changePassword ? this.NewPassword : null
                );

                await _adminApiService.UpdateAsync(_adminId, updateDto);
                Debug.WriteLine("[AdminProfile] API update successful");

                _originalFirstName = FirstName;
                _originalLastName = LastName;
                _originalMiddleName = MiddleName;

                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;
                IsEditMode = false;

                await DialogService.ShowMessage("Данные успешно сохранены.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AdminProfile] ERROR: {ex.Message}");
                await DialogService.ShowError($"Ошибка при сохранении: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }

            Debug.WriteLine("[AdminProfile] Save completed");
            Debug.WriteLine("==========================================");
        }

        private Task CancelAsync()
        {
            Debug.WriteLine("==========================================");
            Debug.WriteLine("[AdminProfile] Cancel EXECUTED");

            FirstName = _originalFirstName;
            LastName = _originalLastName;
            MiddleName = _originalMiddleName;

            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;

            IsEditMode = false;

            Debug.WriteLine("[AdminProfile] Cancel completed");
            Debug.WriteLine("==========================================");

            return Task.CompletedTask;
        }
    }
}