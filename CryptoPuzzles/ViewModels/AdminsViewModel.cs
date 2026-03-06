using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;

namespace CryptoPuzzles.ViewModels
{
    public class AdminsViewModel : EntityViewModelBase<AAdmin, AAdminCreate, AAdminUpdate>
    {
        private string _newPassword = string.Empty;

        public AdminsViewModel(AdminApiService apiService) : base(apiService)
        {
            // Команды инициализируются в базовом классе
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        protected override AAdmin CreateNewItem() => new(0, "", "", "", null, DateTime.Now);
        protected override AAdminCreate MapToCreateDto(AAdmin item) =>
            new(item.Login, NewPassword, item.FirstName, item.LastName, item.MiddleName);
        protected override AAdminUpdate MapToUpdateDto(AAdmin item) =>
            new(item.Id, item.Login, item.FirstName, item.LastName, item.MiddleName,
                string.IsNullOrWhiteSpace(NewPassword) ? null : NewPassword);
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

        protected override async Task DeleteAsync(int? id)
        {
            if (!id.HasValue) return;

            // TODO: добавить проверку на удаление самого себя после реализации ICurrentUserService
            // if (id.Value == текущий_админ) { await DialogService.ShowError(...); return; }

            await base.DeleteAsync(id);
        }

        protected override async Task SaveAsync()
        {
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
                   x.MiddleName == y.MiddleName;
        }

        protected override bool FilterPredicate(AAdmin item)
        {
            if (string.IsNullOrWhiteSpace(FilterText)) return true;
            var f = FilterText.ToLower();
            return item.Login.ToLower().Contains(f) ||
                   item.FirstName.ToLower().Contains(f) ||
                   item.LastName.ToLower().Contains(f) ||
                   (item.MiddleName?.ToLower().Contains(f) ?? false);
        }
    }
}