using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;

namespace CryptoPuzzles.ViewModels
{
    public class AdminsViewModel : EntityViewModelBase<AAdmin, AAdminCreate, AAdminUpdate>
    {
        private string _newPassword = string.Empty;
        private readonly IAuthService _authService;
        private bool _showDeleted = true;
        private string _loginFilter = string.Empty;
        private string _nameFilter = string.Empty;
        private DateTime? _minCreatedAt;
        private DateTime? _maxCreatedAt;

        public AdminsViewModel(AdminApiService apiService, IAuthService authService) : base(apiService)
        {
            _authService = authService;
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public bool ShowDeleted
        {
            get => _showDeleted;
            set
            {
                if (SetProperty(ref _showDeleted, value))
                    ApplyFilter();
            }
        }

        public string LoginFilter
        {
            get => _loginFilter;
            set
            {
                if (SetProperty(ref _loginFilter, value))
                    ApplyFilter();
            }
        }

        public string NameFilter
        {
            get => _nameFilter;
            set
            {
                if (SetProperty(ref _nameFilter, value))
                    ApplyFilter();
            }
        }

        public DateTime? MinCreatedAt
        {
            get => _minCreatedAt;
            set
            {
                if (SetProperty(ref _minCreatedAt, value))
                    ApplyFilter();
            }
        }

        public DateTime? MaxCreatedAt
        {
            get => _maxCreatedAt;
            set
            {
                if (SetProperty(ref _maxCreatedAt, value))
                    ApplyFilter();
            }
        }

        protected override AAdmin CreateNewItem() => new(0, "", "", "", null, DateTime.Now);

        protected override AAdminCreate MapToCreateDto(AAdmin item) =>
            new(item.Login, NewPassword, item.FirstName, item.LastName, item.MiddleName);

        protected override AAdminUpdate MapToUpdateDto(AAdmin item) =>
            new(item.Id, item.Login, item.FirstName, item.LastName, item.MiddleName,
                string.IsNullOrWhiteSpace(NewPassword) ? null : NewPassword, item.IsDeleted, item.DeletedAt);

        protected override int GetId(AAdmin item) => item.Id;

        protected override async Task AddAsync()
        {
            if (string.IsNullOrWhiteSpace(NewItem?.Login))
            {
                await DialogService.ShowError("Логин не может быть пустым!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewItem?.FirstName))
            {
                await DialogService.ShowError("Имя не может быть пустым!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewItem?.LastName))
            {
                await DialogService.ShowError("Фамилия не может быть пустой!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                await DialogService.ShowError("Пароль обязателен при создании!");
                return;
            }

            var itemToAdd = new AAdmin(0, NewItem.Login, NewItem.FirstName, NewItem.LastName,
                NewItem.MiddleName, DateTime.Now);
            Items.Add(itemToAdd);
            _addedItems.Add(itemToAdd);

            NewItem = CreateNewItem();
            NewPassword = string.Empty;
            await Task.CompletedTask;
        }

        protected override async Task SaveAsync()
        {
            var currentAdmin = Items.FirstOrDefault(x => x.Id == _authService.CurrentAdminId);
            if (currentAdmin != null)
            {
                var original = _originalItems.GetValueOrDefault(currentAdmin.Id);
                if (original != null)
                {
                    bool isBeingDeleted = !original.IsDeleted && currentAdmin.IsDeleted;
                    if (isBeingDeleted)
                    {
                        await DialogService.ShowError("Вы не можете удалить самого себя");
                        return;
                    }
                }
            }

            foreach (var item in Items.Except(_addedItems).Where(x => x.Id == _authService.CurrentAdminId))
            {
                if (item.Login != _authService.CurrentAdmin?.Login ||
                    item.FirstName != _authService.CurrentAdmin?.FirstName ||
                    item.LastName != _authService.CurrentAdmin?.LastName ||
                    item.MiddleName != _authService.CurrentAdmin?.MiddleName)
                {
                    var confirm = await DialogService.ShowConfirmation(
                        "Вы изменяете свою учётную запись. Некоторые изменения могут потребовать повторного входа. Продолжить?"
                    );

                    if (!confirm)
                        return;
                }
            }

            foreach (var item in Items.Except(_addedItems))
            {
                if (item.Login == "admin" && _authService.CurrentAdmin?.Login != "admin")
                {
                    await DialogService.ShowError("Вы не можете изменять учётную запись главного администратора");
                    return;
                }
            }

            foreach (var item in _addedItems)
            {
                if (string.IsNullOrWhiteSpace(item.Login) ||
                    string.IsNullOrWhiteSpace(item.FirstName) ||
                    string.IsNullOrWhiteSpace(item.LastName))
                {
                    await DialogService.ShowError("Заполните все обязательные поля (логин, имя, фамилия)!");
                    return;
                }
            }

            foreach (var item in Items.Except(_addedItems))
            {
                if (string.IsNullOrWhiteSpace(item.Login) ||
                    string.IsNullOrWhiteSpace(item.FirstName) ||
                    string.IsNullOrWhiteSpace(item.LastName))
                {
                    await DialogService.ShowError("Заполните все обязательные поля (логин, имя, фамилия)!");
                    return;
                }
            }

            await base.SaveAsync();
            NewPassword = string.Empty;
        }

        protected override bool IsEqual(AAdmin x, AAdmin y)
        {
            return x.Id == y.Id &&
                   x.Login == y.Login &&
                   x.FirstName == y.FirstName &&
                   x.LastName == y.LastName &&
                   x.MiddleName == y.MiddleName &&
                   x.IsDeleted == y.IsDeleted &&
                   x.DeletedAt == y.DeletedAt;
        }

        protected override bool HasAdditionalFilters() =>
            !string.IsNullOrWhiteSpace(LoginFilter) || !string.IsNullOrWhiteSpace(NameFilter) || !ShowDeleted ||
            MinCreatedAt.HasValue || MaxCreatedAt.HasValue;

        protected override bool FilterPredicate(AAdmin item)
        {
            if (!ShowDeleted && item.IsDeleted)
                return false;

            bool loginMatch = string.IsNullOrWhiteSpace(LoginFilter) ||
                              item.Login.Contains(LoginFilter, StringComparison.OrdinalIgnoreCase);
            bool nameMatch = string.IsNullOrWhiteSpace(NameFilter) ||
                             item.FirstName.Contains(NameFilter, StringComparison.OrdinalIgnoreCase) ||
                             item.LastName.Contains(NameFilter, StringComparison.OrdinalIgnoreCase) ||
                             (item.MiddleName != null && item.MiddleName.Contains(NameFilter, StringComparison.OrdinalIgnoreCase));

            bool createdMatch = true;
            if (MinCreatedAt.HasValue)
                createdMatch = item.CreatedAt >= MinCreatedAt.Value;
            if (createdMatch && MaxCreatedAt.HasValue)
                createdMatch = item.CreatedAt <= MaxCreatedAt.Value;

            return loginMatch && nameMatch && createdMatch;
        }
    }
}