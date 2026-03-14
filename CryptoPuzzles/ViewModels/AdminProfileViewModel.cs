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
        private readonly AsyncRelayCommand _saveCommand;

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
            set
            {
                if (SetProperty(ref _newPassword, value))
                    _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
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

        public AdminProfileViewModel(
            AdminApiService adminApiService,
            AAdmin admin,
            Action closeAction)
        {
            _adminApiService = adminApiService ?? throw new ArgumentNullException(nameof(adminApiService));
            _closeAction = closeAction ?? throw new ArgumentNullException(nameof(closeAction));
            _adminId = admin?.Id ?? throw new ArgumentNullException(nameof(admin));

            UpdateFromAdmin(admin);

            CloseCommand = new AsyncRelayCommand(CloseAsync);
            EditCommand = new AsyncRelayCommand(EditAsync, _ => !IsLoading && !IsEditMode);
            _saveCommand = new AsyncRelayCommand(SaveAsync, _ => !IsLoading && IsEditMode && CanSave());
            SaveCommand = _saveCommand;
            CancelCommand = new AsyncRelayCommand(CancelAsync, _ => !IsLoading && IsEditMode);
        }

        private void UpdateFromAdmin(AAdmin admin)
        {
            Login = admin.Login;
            FirstName = admin.FirstName;
            LastName = admin.LastName;
            MiddleName = admin.MiddleName ?? string.Empty;

            _originalFirstName = admin.FirstName;
            _originalLastName = admin.LastName;
            _originalMiddleName = admin.MiddleName ?? string.Empty;
        }

        public async Task LoadAdminDataAsync()
        {
            try
            {
                IsLoading = true;
                var admin = await _adminApiService.GetByIdAsync(_adminId);

                if (admin != null)
                    UpdateFromAdmin(admin);
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка загрузки данных: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanSave()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                return false;

            bool changePassword = !string.IsNullOrWhiteSpace(NewPassword);
            if (changePassword)
            {
                if (NewPassword != ConfirmPassword)
                    return false;
                if (NewPassword.Length < 6)
                    return false;
            }

            return true;
        }

        private Task CloseAsync(object? parameter = null)
        {
            _closeAction?.Invoke();
            return Task.CompletedTask;
        }

        private Task EditAsync(object? parameter = null)
        {
            _originalFirstName = FirstName;
            _originalLastName = LastName;
            _originalMiddleName = MiddleName;

            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;

            IsEditMode = true;

            return Task.CompletedTask;
        }

        private async Task SaveAsync(object? parameter = null)
        {
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

                var updateDto = new AAdminUpdate(
                    Id: _adminId,
                    Login: this.Login,
                    FirstName: this.FirstName,
                    LastName: this.LastName,
                    MiddleName: string.IsNullOrWhiteSpace(this.MiddleName) ? null : this.MiddleName,
                    Password: changePassword ? this.NewPassword : null
                );

                await _adminApiService.UpdateAsync(_adminId, updateDto);

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
                await DialogService.ShowError($"Ошибка при сохранении: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CancelAsync(object? parameter = null)
        {
            FirstName = _originalFirstName;
            LastName = _originalLastName;
            MiddleName = _originalMiddleName;

            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;

            IsEditMode = false;

            await LoadAdminDataAsync();
        }
    }
}