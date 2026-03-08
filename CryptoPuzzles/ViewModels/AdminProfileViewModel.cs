using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class AdminProfileViewModel : ViewModelBase
    {
        private readonly AdminApiService _adminApiService;
        private readonly Action _onClose;
        private readonly int _adminId;

        private string _login;
        private string _firstName;
        private string _lastName;
        private string _middleName;
        private string _newPassword;
        private string _confirmPassword;

        public string Login { get => _login; set => SetProperty(ref _login, value); }
        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
        public string MiddleName { get => _middleName; set => SetProperty(ref _middleName, value); }
        public string NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }
        public string ConfirmPassword { get => _confirmPassword; set => SetProperty(ref _confirmPassword, value); }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AdminProfileViewModel(AdminApiService adminApiService, AAdmin admin, Action onClose)
        {
            _adminApiService = adminApiService;
            _adminId = admin.Id;
            _onClose = onClose;

            Login = admin.Login;
            FirstName = admin.FirstName;
            LastName = admin.LastName;
            MiddleName = admin.MiddleName ?? string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            CancelCommand = new AsyncRelayCommand(_ => { onClose?.Invoke(); return Task.CompletedTask; });
        }

        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Login))
            {
                await DialogService.ShowError("Логин не может быть пустым.");
                return;
            }
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
            if (!string.IsNullOrEmpty(NewPassword) && NewPassword != ConfirmPassword)
            {
                await DialogService.ShowError("Пароли не совпадают.");
                return;
            }

            try
            {
                var updateDto = new AAdminUpdate(
                    Id: _adminId,
                    Login: Login,
                    FirstName: FirstName,
                    LastName: LastName,
                    MiddleName: string.IsNullOrWhiteSpace(MiddleName) ? null : MiddleName,
                    Password: string.IsNullOrWhiteSpace(NewPassword) ? null : NewPassword
                );

                await _adminApiService.UpdateAsync(_adminId, updateDto);
                await DialogService.ShowMessage("Данные успешно обновлены.");
                _onClose?.Invoke();
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}